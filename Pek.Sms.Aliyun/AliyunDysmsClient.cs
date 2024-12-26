using NewLife;
using NewLife.Configuration;

using Pek.Mail;
using Pek.Mail.Client;
using Pek.Sms.Aliyun.Core.Extensions;
using Pek.Sms.Aliyun.Core.Helpers;
using Pek.Sms.Aliyun.Models;
using Pek.Sms.Aliyun.Models.Results;
using Pek.Webs.Clients;

namespace Pek.Sms.Aliyun;

public class AliyunDysmsClient : SmsClientBase
{
    public static String Name { get; } = "aliyun";

    private readonly SmsData _config;
    private readonly WebClient<AliyunDysmsResult> client;
    private String BaseAddress { get; set; }

    public override void CheckMyself() { }

    public AliyunDysmsClient(SmsData? config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        client = new WebClient<AliyunDysmsResult>();

        BaseAddress = config.Security
            ? "https://dysmsapi.aliyuncs.com/"
            : "http://dysmsapi.aliyuncs.com/";
    }

    public async Task<AliyunDysmsResult> SendAsync(AliyunDysmsMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (String.IsNullOrWhiteSpace(_config.AccessKey)) throw new ArgumentNullException(nameof(_config.AccessKey));
        if (String.IsNullOrWhiteSpace(_config.AccessSecret)) throw new ArgumentNullException(nameof(_config.AccessSecret));
        if (String.IsNullOrWhiteSpace(_config.SignName)) throw new ArgumentNullException(nameof(_config.SignName));

        if (message.TemplateCode.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(message.TemplateCode));
        _config.Data["TemplateCode"] = message.TemplateCode;

        message.FixParameters(_config.Data);
        message.CheckParameters();

        var bizParams = new Dictionary<String, Object>
            {
                {"RegionId", "cn-hangzhou"},
                {"Action", "SendSms"},
                {"Version", "2017-05-25"},
                {"AccessKeyId", _config.AccessKey},
                {"PhoneNumbers", message.GetPhoneString()},
                {"SignName", _config.SignName},
                {"TemplateCode", message.TemplateCode},
                {"SignatureMethod", "HMAC-SHA1"},
                {"SignatureNonce", Guid.NewGuid().ToString()},
                {"SignatureVersion", "1.0"},
                {"Timestamp", DateTime.Now.ToIso8601DateString()},
                {"Format", "JSON"}
            };

        if (!String.IsNullOrWhiteSpace(message.OutId))
            bizParams.Add("OutId", message.OutId);

        if (message.HasTemplateParams())
            bizParams.Add("TemplateParam", message.GetTemplateParamsString());

        var signature = SignatureHelper.GetApiSignature(bizParams, _config.AccessSecret);
        bizParams.Add("Signature", signature);

        return await client.Post(BaseAddress)
            .Timeout(_config.Timeout)
            .IgnoreSsl()
            .Retry(_config.RetryTimes)
            .ContentType(HttpContentType.FormUrlEncoded)
            .Data(bizParams)
            .WhenCatch<Exception>(ex =>
            {
                return ReturnAsDefautlResponse(ex.Message);
            })
            .ResultFromJsonAsync().ConfigureAwait(false);
    }

    public async Task<AliyunDysmsResult> SendCodeAsync(AliyunDysmsCode code)
    {
        ArgumentNullException.ThrowIfNull(code);
        if (String.IsNullOrWhiteSpace(_config.AccessKey)) throw new ArgumentNullException(nameof(_config.AccessKey));
        if (String.IsNullOrWhiteSpace(_config.AccessSecret)) throw new ArgumentNullException(nameof(_config.AccessSecret));
        if (String.IsNullOrWhiteSpace(_config.SignName)) throw new ArgumentNullException(nameof(_config.SignName));
        if (code.TemplateCode.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(code.TemplateCode));
        _config.Data["TemplateCode"] = code.TemplateCode;

        code.FixParameters(_config.Data);
        code.CheckParameters();
        code.CheckParameters();

        var bizParams = new Dictionary<String, Object>
            {
                {"RegionId", "cn-hangzhou"},
                {"Action", "SendSms"},
                {"Version", "2017-05-25"},
                {"AccessKeyId", _config.AccessKey},
                {"PhoneNumbers", code.GetPhoneString()},
                {"SignName", _config.SignName},
                {"TemplateCode", code.TemplateCode},
                {"TemplateParam", code.GetTemplateParamsString()},
                {"SignatureMethod", "HMAC-SHA1"},
                {"SignatureNonce", Guid.NewGuid().ToString()},
                {"SignatureVersion", "1.0"},
                {"Timestamp", DateTime.Now.ToIso8601DateString()},
                {"Format", "JSON"}
            };

        if (!String.IsNullOrWhiteSpace(code.OutId))
            bizParams.Add("OutId", code.OutId);

        var signature = SignatureHelper.GetApiSignature(bizParams, _config.AccessSecret);
        bizParams.Add("Signature", signature);

        return await client.Post(BaseAddress)
            .Timeout(_config.Timeout)
            .IgnoreSsl()
            .Retry(_config.RetryTimes)
            .ContentType(HttpContentType.FormUrlEncoded)
            .Data(bizParams)
            .WhenCatch<Exception>(ex =>
            {
                return ReturnAsDefautlResponse(ex.Message);
            })
            .ResultFromJsonAsync().ConfigureAwait(false);
    }

    private static AliyunDysmsResult ReturnAsDefautlResponse(String Message)
            => new()
            {
                RequestId = "",
                Code = "500",
                Message = $"解析错误，返回默认结果:{Message}",
                BizId = ""
            };
}
