using NewLife;

using Pek.Mail;
using Pek.Mail.Client;
using Pek.Mail.Exceptions;
using Pek.Sms.Aliyun.Core.Extensions;
using Pek.Sms.Aliyun.Core.Helpers;
using Pek.Sms.Aliyun.Models;
using Pek.Sms.Aliyun.Models.Results;

namespace Pek.Sms.Aliyun;

public class AliyunDysmsClient : SmsClientBase
{
    private readonly Action<Exception>? _exceptionHandler;

    public override void CheckMyself() { }

    public AliyunDysmsClient(Action<Exception>? exceptionHandler = null)
    {
        var globalHandle = ExceptionHandleResolver.ResolveHandler();
        globalHandle += exceptionHandler;
        _exceptionHandler = globalHandle;
    }

    public async Task<AliyunDysmsResult> SendAsync(SmsData config, AliyunDysmsMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (String.IsNullOrWhiteSpace(config.AccessKey)) throw new ArgumentNullException(nameof(config.AccessKey));
        if (String.IsNullOrWhiteSpace(config.AccessSecret)) throw new ArgumentNullException(nameof(config.AccessSecret));
        if (String.IsNullOrWhiteSpace(config.SignName)) throw new ArgumentNullException(nameof(config.SignName));
        message.FixParameters(config.Data);

        if (config.Data?["TemplateCode"].SafeString().IsNullOrWhiteSpace() == true) throw new ArgumentNullException("TemplateCode");
        message.FixParameters(config.Data);

        message.CheckParameters();

        var bizParams = new Dictionary<String, String>
            {
                {"RegionId", "cn-hangzhou"},
                {"Action", "SendSms"},
                {"Version", "2017-05-25"},
                {"AccessKeyId", config.AccessKey},
                {"PhoneNumbers", message.GetPhoneString()},
                {"SignName", config.SignName},
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

        var signature = SignatureHelper.GetApiSignature(bizParams, config.AccessSecret);
        bizParams.Add("Signature", signature);

        var content = new FormUrlEncodedContent(bizParams);

        //var client = new Pek.Webs.Clients.WebClient();
        //client.Post("").ResultFromJsonAsync<AliyunDysmsResult>();

        //return await _proxy.SendMessageAsync(content)
        //    .Retry(config.Data?["RetryTimes"].SafeString())
        //    .Handle()
        //    .WhenCatch<Exception>(e =>
        //    {
        //        _exceptionHandler?.Invoke(e);
        //        return ReturnAsDefautlResponse();
        //    });

        return null;
    }

    //public async Task<AliyunDysmsResult> SendCodeAsync(AliyunDysmsCode code)
    //{
    //    ArgumentNullException.ThrowIfNull(code);
    //    if (String.IsNullOrWhiteSpace(_aliyunDysmsAccount.AccessKeyId)) throw new ArgumentNullException(nameof(_aliyunDysmsAccount.AccessKeyId));
    //    if (String.IsNullOrWhiteSpace(_aliyunDysmsAccount.AccessKeySecret)) throw new ArgumentNullException(nameof(_aliyunDysmsAccount.AccessKeySecret));
    //    if (String.IsNullOrWhiteSpace(_config.SignName)) throw new ArgumentNullException(nameof(_config.SignName));
    //    code.FixParameters(_config);
    //    code.CheckParameters();

    //    var bizParams = new Dictionary<string, string>
    //        {
    //            {"RegionId", "cn-hangzhou"},
    //            {"Action", "SendSms"},
    //            {"Version", "2017-05-25"},
    //            {"AccessKeyId", _aliyunDysmsAccount.AccessKeyId},
    //            {"PhoneNumbers", code.GetPhoneString()},
    //            {"SignName", _config.SignName},
    //            {"TemplateCode", code.TemplateCode},
    //            {"TemplateParam", code.GetTemplateParamsString()},
    //            {"SignatureMethod", "HMAC-SHA1"},
    //            {"SignatureNonce", Guid.NewGuid().ToString()},
    //            {"SignatureVersion", "1.0"},
    //            {"Timestamp", DateTime.Now.ToIso8601DateString()},
    //            {"Format", "JSON"}
    //        };

    //    if (!String.IsNullOrWhiteSpace(code.OutId))
    //        bizParams.Add("OutId", code.OutId);

    //    var signature = SignatureHelper.GetApiSignature(bizParams, _aliyunDysmsAccount.AccessKeySecret);
    //    bizParams.Add("Signature", signature);

    //    var content = new FormUrlEncodedContent(bizParams);

    //    return await _proxy.SendCodeAsync(content)
    //        .Retry(_config.RetryTimes)
    //        .Handle()
    //        .WhenCatch<Exception>(e =>
    //        {
    //            _exceptionHandler?.Invoke(e);
    //            return ReturnAsDefautlResponse();
    //        });
    //}

    private static AliyunDysmsResult ReturnAsDefautlResponse()
            => new()
            {
                RequestId = "",
                Code = "500",
                Message = "解析错误，返回默认结果",
                BizId = ""
            };
}
