using System.Text.Json;

using NewLife;
using NewLife.Log;
using NewLife.Serialization;

using Pek.Ids;
using Pek.Security;
using Pek.Webs.Clients;

namespace Pek.Sms.FengHuo;

public class FengHuoSmsClient
{
    public static String Name { get; } = "fenghuo";

    private readonly SmsData _config;
    private readonly SmsSettings _settings;
    private String BaseAddress { get; set; }
    private readonly WebClient _client;

    public FengHuoSmsClient(SmsData? config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _settings = SmsSettings.Current;
        _client = new WebClient();

        // 新版 API 基础地址，需要从配置中获取，默认使用旧地址兼容
        var baseUrl = _config.Data.TryGetValue("BaseUrl", out var url) && url != null
            ? url.ToString()
            : "https://aisms.aipaas.com:8443";

        BaseAddress = baseUrl!;

        // 确保以 /sms 结尾
        if (!BaseAddress.EndsWith("/sms"))
        {
            BaseAddress = BaseAddress.TrimEnd('/') + "/sms";
        }
    }

    #region 辅助方法
    /// <summary>获取当前时间戳，精确到毫秒</summary>
    private static Int64 GetTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>计算签名，规则：MD5(userName + timestamp + MD5(password))</summary>
    /// <param name="userName">用户名</param>
    /// <param name="timestamp">时间戳</param>
    /// <param name="password">密码</param>
    private static String CalculateSign(String userName, Int64 timestamp, String password)
    {
        var passwordMd5 = Encrypt.MDString(password).ToLower();
        var signStr = userName + timestamp + passwordMd5;
        return Encrypt.MDString(signStr).ToLower();
    }
    #endregion

    #region 发送短信
    /// <summary>发送短信</summary>
    /// <param name="mobiles">手机号，可批量，用逗号分隔开，上限为10000个</param>
    /// <param name="content">短信内容，与短信模板ID必传其一</param>
    /// <param name="templateId">短信模板ID，与短信内容必传其一</param>
    /// <param name="paramValues">模板参数值数组</param>
    /// <param name="sendTime">短信定时发送时间，格式：yyyy-MM-dd HH:mm:ss，定时时间限制15天以内</param>
    /// <param name="extcode">附带通道扩展码</param>
    /// <param name="callData">用户回传数据，最大长度64，在回执推送时回传</param>
    public async Task<SmsResult> SendAsync(String mobiles, String? content = null, Int32? templateId = null, String[]? paramValues = null, String? callData = null, String? sendTime = null, String? extcode = null)
    {
        ArgumentNullException.ThrowIfNull(mobiles);
        if (String.IsNullOrWhiteSpace(_config.AccessKey)) throw new ArgumentNullException(nameof(_config.AccessKey));
        if (String.IsNullOrWhiteSpace(_config.AccessSecret)) throw new ArgumentNullException(nameof(_config.AccessSecret));

        // 验证 content 和 templateId 至少有一个
        if (String.IsNullOrWhiteSpace(content) && !templateId.HasValue)
        {
            return new SmsResult(false, "短信内容和模板ID不能同时为空");
        }

        // 解析手机号列表
        var phoneList = mobiles.Split([',', ';', '，', '；'], StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim())
            .Where(p => !String.IsNullOrWhiteSpace(p))
            .Distinct()
            .ToList();

        if (phoneList.Count == 0)
        {
            return new SmsResult(false, "手机号列表为空");
        }

        // 将参数数组转换为字典（新版API使用变量名映射）
        Dictionary<String, String>? paramsDict = null;
        if (paramValues != null && paramValues.Length > 0)
        {
            paramsDict = new Dictionary<String, String>();
            for (var i = 0; i < paramValues.Length; i++)
            {
                paramsDict[$"param{i + 1}"] = paramValues[i];
            }
        }

        // 构建 JSON 请求对象
        var timestamp = GetTimestamp();
        var requestData = new
        {
            userName = _config.AccessKey,
            timestamp,
            sign = CalculateSign(_config.AccessKey, timestamp, _config.AccessSecret),
            content = String.IsNullOrWhiteSpace(content) ? String.Empty : content,
            templateId = templateId,
            @params = paramsDict,
            phoneList,
            sendTime = String.IsNullOrWhiteSpace(sendTime) ? String.Empty : sendTime,
            extcode = String.IsNullOrWhiteSpace(extcode) ? String.Empty : extcode,
            callData = String.IsNullOrWhiteSpace(callData) ? String.Empty : callData
        };

        XTrace.WriteLine($"打印请求对象：{requestData.ToJson()}");

        return await SendMessageMassAsync(requestData).ConfigureAwait(false);
    }
    #endregion

    #region 核心接口实现
    /// <summary>短信批量发送接口</summary>
    /// <param name="requestData">请求参数</param>
    private async Task<SmsResult> SendMessageMassAsync(Object requestData)
    {
        var result = String.Empty;

        try
        {
            IHttpRequest request;

            // 根据配置判断是否使用代理（本地调试时方便访问限制IP的平台）
            if (_settings.EnableProxy && !_settings.ProxyUrl.IsNullOrWhiteSpace())
            {
                // 使用代理模式
                request = _client.Post($"{_settings.ProxyUrl}/api/sendMessageMass")
                    .ContentType(HttpContentType.Json)
                    .Header("X-Target-Url", BaseAddress)
                    .Header("Id", IdHelper.GetNextId());

                // 添加代理认证令牌
                if (!_settings.ProxyToken.IsNullOrWhiteSpace())
                {
                    request.Header("X-Token-Code", _settings.ProxyToken);
                }

                request.JsonData(requestData);
            }
            else
            {
                // 直接请求
                request = _client.Post($"{BaseAddress}/api/sendMessageMass")
                    .ContentType(HttpContentType.Json)
                    .JsonData(requestData);
            }

            var response = await request
                .Timeout(_config.Timeout)
                .IgnoreSsl()
                .Retry(_config.RetryTimes)
                .GetResponseAsync()
                .ConfigureAwait(false);

            result = response.Data ?? String.Empty;
            XTrace.WriteLine($"短信发送返回：{result}");

            // 解析 JSON 响应
            using var doc = JsonDocument.Parse(result);
            var root = doc.RootElement;

            var code = root.GetProperty("code").GetInt32();
            var message = root.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : String.Empty;

            if (code == 0)
            {
                var msgId = root.TryGetProperty("msgId", out var idProp) ? idProp.GetInt64() : 0;
                var smsCount = root.TryGetProperty("smsCount", out var countProp) ? countProp.GetInt32() : 0;
                return new SmsResult(true, $"发送成功，消息ID：{msgId}，计费条数：{smsCount}");
            }
            else
            {
                return new SmsResult(false, $"错误码：{code}，{message}。原始响应：{result}");
            }
        }
        catch (Exception ex)
        {
            return new SmsResult(false, $"发送异常：{ex.Message}：{result}");
        }
    }
    #endregion
}
