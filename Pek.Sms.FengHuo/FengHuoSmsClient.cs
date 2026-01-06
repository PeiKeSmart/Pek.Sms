using Pek.Mail;
using Pek.Security;
using Pek.Webs.Clients;

namespace Pek.Sms.FengHuo;

public class FengHuoSmsClient
{
    public static String Name { get; } = "fenghuo";

    private readonly SmsData _config;
    private String BaseAddress { get; set; }
    private readonly WebClient client;

    public FengHuoSmsClient(SmsData? config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        client = new WebClient();

        BaseAddress = config.Security
            ? "https://51sms.aipaas.com/sms/"
            : "http://51sms.aipaas.com/sms/";
    }

    /// <summary>
    /// 获取YYYYMMDDHHMISS格式当前时间
    /// </summary>
    /// <returns></returns>
    private static String GetSeed() => DateTime.Now.ToString("yyyyMMddHHmmss");

    /// <summary>
    /// 获取加密数据
    /// </summary>
    /// <param name="seed">时间</param>
    /// <returns></returns>
    private String GetToken(String seed) => Encrypt.Sha1("account=" + _config.AccessKey + "&ts=" + seed + "&secret=" + _config.AccessSecret).ToLower();

    /// <summary>
    /// 发送短信
    /// </summary>
    /// <param name="mobile">手机号,可批量，用逗号分隔开，上限为1000个</param>
    /// <param name="content">内容</param>
    public async Task<SmsResult> SendAsync(String mobile, String content)
    {
        ArgumentNullException.ThrowIfNull(mobile);
        if (String.IsNullOrWhiteSpace(_config.AccessKey)) throw new ArgumentNullException(nameof(_config.AccessKey));
        if (String.IsNullOrWhiteSpace(_config.AccessSecret)) throw new ArgumentNullException(nameof(_config.AccessSecret));
        if (String.IsNullOrWhiteSpace(_config.SignName)) throw new ArgumentNullException(nameof(_config.SignName));

        var seed = GetSeed();
        var token = GetToken(seed);
        var sendaction = BaseAddress + "send";

        String result;
        try
        {
            var response = await client.Post(sendaction)
                .Timeout(_config.Timeout)
                .IgnoreSsl()
                .Retry(_config.RetryTimes)
                .Data("account", _config.AccessKey)
                .Data("token", token)
                .Data("ts", seed)
                .Data("mobiles", mobile)
                .Data("content", content.UrlEncode())
                .Data("ext", "")
                .GetResponseAsync().ConfigureAwait(false);
            result = response.Data ?? String.Empty;
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        if (result.Contains("提交成功"))
        {
            return new SmsResult(true, result);
        }
        else
        {
            return new SmsResult(false, result);
        }
    }

    /// <summary>
    /// 发送模板短信
    /// </summary>
    /// <param name="mobiles">手机号,可批量，用逗号分隔开，上限为1000个</param>
    /// <param name="templateId">对应的模板ID</param>
    /// <param name="paramValues">对应的参数</param>
    /// <param name="Ref"></param>
    /// <returns></returns>
    public async Task<SmsResult> SendTemplateParamd(String mobiles, String templateId, String[] paramValues, Object Ref)
    {
        ArgumentNullException.ThrowIfNull(mobiles);
        ArgumentNullException.ThrowIfNull(templateId);
        if (String.IsNullOrWhiteSpace(_config.AccessKey)) throw new ArgumentNullException(nameof(_config.AccessKey));
        if (String.IsNullOrWhiteSpace(_config.AccessSecret)) throw new ArgumentNullException(nameof(_config.AccessSecret));
        if (String.IsNullOrWhiteSpace(_config.SignName)) throw new ArgumentNullException(nameof(_config.SignName));

        var seed = GetSeed();
        var token = GetToken(seed);
        var sendaction = BaseAddress + "sendTemplateParamd";

        var irequest = client.Post(sendaction)
            .Timeout(_config.Timeout)
            .IgnoreSsl()
            .Retry(_config.RetryTimes)
            .Data("account", _config.AccessKey)
            .Data("token", token)
            .Data("ts", seed)
            .Data("templateId", templateId)
            .Data("mobiles", mobiles)
            .Data("ref", Ref)
            .Data("ext", "");

        for (var i = 0; i < paramValues.Length; i++)
        {
            irequest.Data($"param{i + 1}", paramValues[i]);
        }

        String result;
        try
        {
            var response = await irequest.GetResponseAsync().ConfigureAwait(false);
            result = response.Data ?? String.Empty;
        }
        catch (Exception ex)
        {
            result = ex.Message;
        }

        if (result.Contains("提交成功"))
        {
            return new SmsResult(true, result);
        }
        else
        {
            return new SmsResult(false, result);
        }
    }
}
