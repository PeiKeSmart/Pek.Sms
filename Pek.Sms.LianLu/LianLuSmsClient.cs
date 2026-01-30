using Pek.Mail;
using Pek.Security;
using Pek.Timing;
using Pek.Webs.Clients;

namespace Pek.Sms.LianLu;

public class LianLuSmsClient
{
    public static String Name { get; } = "lianlu";

    private readonly SmsData _config;
    private String BaseAddress { get; set; }
    private readonly WebClient client;

    public LianLuSmsClient(SmsData? config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        client = new WebClient();

        BaseAddress = config.Security
            ? "http://47.110.199.86:8081"
            : "http://47.110.199.86:8081";
    }

    /// <summary>
    /// 发送短信
    /// </summary>
    /// <param name="mobiles">手机号,可批量，用逗号分隔开，上限为1000个</param>
    /// <param name="content">内容</param>
    public async Task<SmsResult> SendAsync(String mobiles, String content)
    {
        ArgumentNullException.ThrowIfNull(mobiles);
        if (String.IsNullOrWhiteSpace(_config.AccessKey)) throw new ArgumentNullException(nameof(_config.AccessKey));
        if (String.IsNullOrWhiteSpace(_config.AccessSecret)) throw new ArgumentNullException(nameof(_config.AccessSecret));
        if (String.IsNullOrWhiteSpace(_config.SignName)) throw new ArgumentNullException(nameof(_config.SignName));

        var sendaction = BaseAddress + "/api/sms/send";

        var ts = UnixTime.ToTimestamp();
        var sign = Encrypt.GetMD5($"{_config.AccessKey}{ts}{_config.AccessSecret}").ToLower();

        String result;
        try
        {
            var response = await client.Post(sendaction)
                .Timeout(_config.Timeout)
                .IgnoreSsl()
                .Retry(_config.RetryTimes)
                .Data("userid", _config.AccessKey)
                .Data("ts", ts)
                .Data("sign", sign)
                .Data("mobile", mobiles)
                //.Data("msgcontent", content.UrlEncode())
                .Data("msgcontent", content)
                .Data("extnum", "")
                .Data("time", "")
                .Data("messageid", "")
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
}
