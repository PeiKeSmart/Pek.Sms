using NewLife;

using Pek.Events;
using Pek.Helpers;
using Pek.Sms.Events;

namespace Pek.Sms.FengHuo.Events;

/// <summary>
/// 短信事件消费者
/// </summary>
public class SmsEventConsumer : IConsumer<SmsEvent>
{
    public Int32 Sort { get; set; } = SmsSettings.Current.FindByName(FengHuoSmsClient.Name)?.FirstOrDefault(e => e.SmsType == 0)?.Order ?? 10;

    public async Task HandleEventAsync(SmsEvent eventMessage)
    {
        if (eventMessage.Success) return;

        var list = SmsSettings.Current.FindByName(FengHuoSmsClient.Name);

        if (list == null) return;

        var model = list.FirstOrDefault(e => e.SmsType == 0);
        if (model == null) return;
        if (!model.IsEnabled) return;

        var mobiles = eventMessage.Data?["mobiles"]?.SafeString();
        if (mobiles.IsNullOrWhiteSpace()) return; 

        var content = eventMessage.Data?["content"]?.SafeString();
        var templateId = eventMessage.Data?["templateId"]?.ToDGInt();
        var paramValues = eventMessage.Data?["paramValues"] as IDictionary<String, String>;
        var callData = eventMessage.Data?["callData"]?.SafeString();
        var sendTime = eventMessage.Data?["sendTime"]?.SafeString();
        var extcode = eventMessage.Data?["extcode"]?.SafeString();

        var client = new FengHuoSmsClient(model);

        var resultSms = await client.SendAsync(
                        mobiles,
                        content,
                        templateId,
                        paramValues,
                        callData,
                        sendTime,
                        extcode
                        ).ConfigureAwait(false);  //发送短信

        if (!resultSms.Success)
        {
            eventMessage.Success = false;
            eventMessage.Message = "发送失败";
            return;
        }

        eventMessage.Success = true;
        eventMessage.Result = resultSms;
    }
}
