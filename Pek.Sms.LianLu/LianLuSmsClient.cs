using Pek.Mail;
using Pek.Timing;
using Pek.Webs.Clients;

namespace Pek.Sms.LianLu;

public class LianLuSmsClient
{
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
    public async Task<SmsResult> SendAsync(string mobiles, string content)
    {
        var sendaction = BaseAddress + "/api/sms/send";

        var ts = UnixTime.ToTimestamp();
        var sign = Encrypt.GetMD5($"{_options.AccessKeyId}{ts}{_options.AccessKeySecret}").ToLower();

        var result = await Pek.Helpers.DHWeb.Client().Post(sendaction)
            .Data("userid", _options.AccessKeyId)
            .Data("ts", ts)
            .Data("sign", sign)
            .Data("mobile", mobiles)
            //.Data("msgcontent", content.UrlEncode())
            .Data("msgcontent", content)
            .Data("extnum", "")
            .Data("time", "")
            .Data("messageid", "")
            .ResultAsync();

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
