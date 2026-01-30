using Pek.Events;
using Pek.Sms.Events;

namespace Pek.Sms.LianLu.Events;

public class SmsEventConsumer1 : IConsumer<SmsEvent1>
{
    public Int32 Sort { get; set; } = 20;

    public async Task HandleEventAsync(SmsEvent1 eventMessage)
    {
        if (eventMessage.Success) return;

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
