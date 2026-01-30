using Pek.Events;
using Pek.Sms.Events;

namespace Pek.Sms.FengHuo.Events;

public class SmsEventConsumer1 : IConsumer<SmsEvent1>
{
    public Int32 Sort { get; set; } = SmsSettings.Current.FindByName(FengHuoSmsClient.Name)?.FirstOrDefault().Order ?? 10;

    public async Task HandleEventAsync(SmsEvent1 eventMessage)
    {
        if (eventMessage.Success) return;

        var list = SmsSettings.Current.FindByName(FengHuoSmsClient.Name);

        if (list == null) return;

        var model = list.FirstOrDefault(e => e.SmsType == eventMessage.SmsType);

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
