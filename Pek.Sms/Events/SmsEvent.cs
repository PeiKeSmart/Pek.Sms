namespace Pek.Sms.Events;

/// <summary>
/// 短信消费者事件
/// </summary>
public class SmsEvent
{
    public SmsEvent(Int32 smsType)
    {
        SmsType = smsType;
    }

    public Int32 SmsType { get; set; }

    public Boolean Success { get; set; }
}
