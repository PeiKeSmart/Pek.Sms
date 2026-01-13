using NewLife;
using NewLife.Serialization;

using Pek.Mail;
using Pek.Sms.TencentCloud.Core;
using Pek.Sms.TencentCloud.Models.Results;
using Pek.Timing;
using Pek.Webs.Clients;

namespace Pek.Sms.TencentCloud;

/// <summary>腾讯云短信客户端</summary>
public class TencentSmsClient
{
    public static String Name { get; } = "tencent";

    private readonly SmsData _config;
    private readonly WebClient _client;
    private const String BaseAddress = "https://sms.tencentcloudapi.com";
    private const String ApiVersion = "2021-01-11";
    private const String Region = "ap-guangzhou";

    /// <summary>实例化腾讯云短信客户端</summary>
    /// <param name="config">配置</param>
    public TencentSmsClient(SmsData? config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _client = new WebClient();
    }

    /// <summary>发送短信</summary>
    /// <param name="mobiles">手机号，多个以逗号分隔</param>
    /// <param name="templateParams">模板参数数组</param>
    /// <param name="templateId">模板ID，为空时从配置读取</param>
    /// <param name="extendCode">短信码号扩展号</param>
    /// <param name="sessionContext">用户的session内容</param>
    /// <param name="senderId">国际/港澳台短信SenderId</param>
    /// <returns>发送结果</returns>
    public async Task<TencentSmsResult> SendAsync(String mobiles, String[]? templateParams = null, String? templateId = null, String? extendCode = null, String? sessionContext = null, String? senderId = null)
    {
        if (String.IsNullOrWhiteSpace(mobiles)) throw new ArgumentNullException(nameof(mobiles));
        if (String.IsNullOrWhiteSpace(_config.AccessKey)) throw new ArgumentNullException(nameof(_config.AccessKey), "SecretId不能为空");
        if (String.IsNullOrWhiteSpace(_config.AccessSecret)) throw new ArgumentNullException(nameof(_config.AccessSecret), "SecretKey不能为空");
        if (String.IsNullOrWhiteSpace(_config.SignName)) throw new ArgumentNullException(nameof(_config.SignName), "签名不能为空");

        // 从配置获取参数
        var sdkAppId = _config.Data?["SdkAppId"]?.ToString();
        if (String.IsNullOrWhiteSpace(sdkAppId)) throw new InvalidOperationException("SdkAppId不能为空");

        templateId = templateId ?? _config.Data?["TemplateId"]?.ToString();
        if (String.IsNullOrWhiteSpace(templateId)) throw new InvalidOperationException("TemplateId不能为空");

        // 获取国家码，默认+86（中国大陆）
        var countryCode = _config.Data?["CountryCode"]?.ToString() ?? "+86";

        // 处理手机号
        var phoneArray = mobiles.Split([',', ';', '|'], StringSplitOptions.RemoveEmptyEntries)
                                .Select(p =>
                                {
                                    var phone = p.Trim();
                                    // 如果已经包含+号，直接使用
                                    if (phone.StartsWith("+")) return phone;
                                    // 否则添加国家码
                                    return $"{countryCode}{phone}";
                                })
                                .ToArray();

        // 构建请求体
        var requestBody = new
        {
            SdkAppId = sdkAppId,
            SignName = _config.SignName,
            TemplateId = templateId,
            PhoneNumberSet = phoneArray,
            TemplateParamSet = templateParams ?? [],
            ExtendCode = extendCode,
            SessionContext = sessionContext,
            SenderId = senderId
        };

        var payload = requestBody.ToJson();
        var timestamp = UnixTime.ToTimestamp();

        // 生成签名
        var authorization = TencentSignatureHelper.GenerateAuthorization(
            _config.AccessKey,
            _config.AccessSecret,
            payload,
            timestamp
        );

        try
        {
            // 发送请求
            var response = await _client.Post(BaseAddress)
                .Timeout(_config.Timeout)
                .IgnoreSsl()
                .Retry(_config.RetryTimes)
                .ContentType(HttpContentType.Json)
                .Header("X-TC-Action", "SendSms")
                .Header("X-TC-Region", Region)
                .Header("X-TC-Timestamp", timestamp.ToString())
                .Header("X-TC-Version", ApiVersion)
                .Header("Authorization", authorization)
                .JsonData(payload)
                .GetResponseAsync().ConfigureAwait(false);

            return ParseResponse(response.Data ?? String.Empty);
        }
        catch (Exception ex)
        {
            return new TencentSmsResult
            {
                IsSuccess = false,
                Code = "ClientError",
                Message = $"请求异常：{ex.Message}"
            };
        }
    }

    /// <summary>解析响应</summary>
    private static TencentSmsResult ParseResponse(String responseBody)
    {
        try
        {
            var cloudResponse = responseBody.ToJsonEntity<TencentCloudResponse>();
            if (cloudResponse?.Response == null)
            {
                return new TencentSmsResult
                {
                    IsSuccess = false,
                    Message = "响应数据为空"
                };
            }

            var response = cloudResponse.Response;

            // 检查是否有错误
            if (response.Error != null)
            {
                return new TencentSmsResult
                {
                    IsSuccess = false,
                    RequestId = response.RequestId,
                    Code = response.Error.Code,
                    Message = response.Error.Message
                };
            }

            // 检查发送状态
            var isSuccess = response.SendStatusSet?.All(s => s.Code == "Ok") ?? false;

            return new TencentSmsResult
            {
                IsSuccess = isSuccess,
                RequestId = response.RequestId,
                SendStatusSet = response.SendStatusSet,
                Code = isSuccess ? "Ok" : response.SendStatusSet?.FirstOrDefault()?.Code,
                Message = isSuccess ? "发送成功" : response.SendStatusSet?.FirstOrDefault()?.Message
            };
        }
        catch (Exception ex)
        {
            return new TencentSmsResult
            {
                IsSuccess = false,
                Message = $"解析响应失败：{ex.Message}"
            };
        }
    }
}
