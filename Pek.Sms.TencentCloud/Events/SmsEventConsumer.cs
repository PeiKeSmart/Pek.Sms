using Pek.Events;
using Pek.Sms.Events;

namespace Pek.Sms.TencentCloud.Events;

public class SmsEventConsumer : IConsumer<SmsEvent>
{
    public Int32 Sort { get; set; } = 20;

    public async Task HandleEventAsync(SmsEvent eventMessage)
    {
        if (eventMessage.Success) return;

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
