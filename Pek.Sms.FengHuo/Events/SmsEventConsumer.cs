using Pek.Events;
using Pek.Sms.Events;

namespace Pek.Sms.FengHuo.Events;

public class SmsEventConsumer : IConsumer<SmsEvent>
{
    public Int32 Sort { get; set; } = SmsSettings.Current.FindByName(FengHuoSmsClient.Name)?.FirstOrDefault(e => e.SmsType == 1)?.Order ?? 10;

    public async Task HandleEventAsync(SmsEvent eventMessage)
    {
        if (eventMessage.Success) return;

        var list = SmsSettings.Current.FindByName(FengHuoSmsClient.Name);

        if (list == null) return;

        var model = list.FirstOrDefault(e => e.SmsType == 1);
        if (model == null) return;
        if (!model.IsEnabled) return;



        await Task.CompletedTask.ConfigureAwait(false);
    }
}
