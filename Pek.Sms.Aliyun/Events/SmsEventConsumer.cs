using NewLife;
using NewLife.Serialization;

using Pek.Events;
using Pek.Sms.Aliyun.Models;
using Pek.Sms.Events;

namespace Pek.Sms.Aliyun.Events;

/// <summary>
/// 短信事件消费者
/// </summary>
public class SmsEventConsumer : IConsumer<SmsEvent>
{
    public Int32 Sort { get; set; } = SmsSettings.Current.FindByName(AliyunDysmsClient.Name)?.FirstOrDefault(e => e.SmsType == 0)?.Order ?? 20;

    public async Task HandleEventAsync(SmsEvent eventMessage)
    {
        if (eventMessage.Success) return;

        var list = SmsSettings.Current.FindByName(AliyunDysmsClient.Name);

        if (list == null) return;

        var model = list.FirstOrDefault(e => e.SmsType == 0);
        if (model == null) return;
        if (!model.IsEnabled) return;

        var Phone = eventMessage.Data?["Phone"]?.SafeString();
        if (Phone.IsNullOrWhiteSpace()) return;

        var TemplateCode = eventMessage.Data?["TemplateCode"]?.SafeString();
        if (TemplateCode.IsNullOrWhiteSpace()) return;

        var TemplateParams = eventMessage.Data?["TemplateParams"] as Dictionary<String, String>;
        if (TemplateParams == null) return;

        var message = new AliyunDysmsMessage
        {
            Phone = [.. Phone.Split(',', StringSplitOptions.RemoveEmptyEntries)],
            TemplateCode = TemplateCode,
            TemplateParams = TemplateParams
        };

        var client = new AliyunDysmsClient(model);

        var resultSms = await client.SendAsync(message).ConfigureAwait(false);  //发送短信

        if (!resultSms.IsSuccess())
        {
            eventMessage.Success = false;
            eventMessage.Message = "发送失败";
            return;
        }

        eventMessage.Success = true;
        eventMessage.Result = new SmsResult(true, resultSms.Message, resultSms.ToJson());
    }
}