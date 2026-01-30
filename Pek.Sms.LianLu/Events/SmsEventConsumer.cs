using NewLife;

using Pek;
using Pek.Events;
using Pek.Sms;

using Pek.Sms.Events;
using Pek.Sms.LianLu;

/// <summary>
/// 短信事件消费者
/// </summary>
public class SmsEventConsumer : IConsumer<SmsEvent>
{
    public Int32 Sort { get; set; } = SmsSettings.Current.FindByName(LianLuSmsClient.Name)?.FirstOrDefault(e => e.SmsType == 0)?.Order ?? 40;

    public async Task HandleEventAsync(SmsEvent eventMessage)
    {
        if (eventMessage.Success) return;

        var list = SmsSettings.Current.FindByName(LianLuSmsClient.Name);

        if (list == null) return;

        var model = list.FirstOrDefault(e => e.SmsType == 0);
        if (model == null) return;
        if (!model.IsEnabled) return;

        var mobiles = eventMessage.Data?["mobiles"]?.SafeString();
        if (mobiles.IsNullOrWhiteSpace()) return;

        var content = eventMessage.Data?["content"]?.SafeString();
        if (content.IsNullOrWhiteSpace()) return;

        var client = new LianLuSmsClient(model);

        var resultSms = await client.SendAsync(
                        mobiles,
                        content
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