namespace Pek.Sms.Events;

/// <summary>
/// 短信消费者事件
/// </summary>
public class SmsEvent2
{
    public SmsEvent2()
    {
        SmsType = 2;
    }

    public Int32 SmsType { get; set; }

    public Boolean Success { get; set; }
}
