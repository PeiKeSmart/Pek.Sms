using NewLife;
using NewLife.Serialization;

using Pek.Events;
using Pek.Sms.Events;

namespace Pek.Sms.TencentCloud.Events;

/// <summary>
/// 短信事件消费者
/// </summary>
public class SmsEventConsumer : IConsumer<SmsEvent>
{
    public Int32 Sort { get; set; } = SmsSettings.Current.FindByName(TencentSmsClient.Name)?.FirstOrDefault(e => e.SmsType == 0)?.Order ?? 30;

    public async Task HandleEventAsync(SmsEvent eventMessage)
    {
        if (eventMessage.Success) return;

        var list = SmsSettings.Current.FindByName(TencentSmsClient.Name);

        if (list == null) return;

        var model = list.FirstOrDefault(e => e.SmsType == 0);
        if (model == null) return;
        if (!model.IsEnabled) return;

        var mobiles = eventMessage.Data?["mobiles"]?.SafeString();
        if (mobiles.IsNullOrWhiteSpace()) return;

        var paramValues = eventMessage.Data?["paramValues"] as IDictionary<String, String>;
        var templateParams = paramValues?.Values.ToArray();

        var templateId = eventMessage.Data?["templateId"]?.SafeString();
        var extcode = eventMessage.Data?["extcode"]?.SafeString();
        var sessionContext = eventMessage.Data?["sessionContext"]?.SafeString();
        var senderId = eventMessage.Data?["senderId"]?.SafeString();

        var client = new TencentSmsClient(model);

        var resultSms = await client.SendAsync(
                        mobiles,
                        templateParams,
                        templateId,
                        extcode,
                        sessionContext,
                        senderId
                    ).ConfigureAwait(false);

        if (!resultSms.IsSuccess)
        {
            eventMessage.Success = false;
            eventMessage.Message = "发送失败";
            return;
        }

        eventMessage.Success = true;
        eventMessage.Result = new SmsResult(true, resultSms.Message, resultSms.ToJson());
    }
}