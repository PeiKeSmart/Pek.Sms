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

        var mobiles = eventMessage.Data?["mobiles"]?.SafeString();
        if (mobiles.IsNullOrWhiteSpace()) return;

        var templateId = eventMessage.Data?["templateId"]?.SafeString();
        if (templateId.IsNullOrWhiteSpace()) return;

        var paramValues = eventMessage.Data?["paramValues"] as Dictionary<String, String>;
        if (paramValues == null) return;

        var message = new AliyunDysmsMessage
        {
            Phone = [.. mobiles.Split(',', StringSplitOptions.RemoveEmptyEntries)],
            TemplateCode = templateId,
            TemplateParams = paramValues
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