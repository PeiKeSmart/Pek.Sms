﻿using Pek.Events;
using Pek.Sms.Events;

namespace Pek.Sms.Aliyun.Events;

public class SmsEventConsumer : IConsumer<SmsEvent>
{
    public Int32 Sort { get; set; } = 20;

    public async Task HandleEventAsync(SmsEvent eventMessage)
    {

        await Task.CompletedTask.ConfigureAwait(false);
    }
}
