namespace Pek.Sms.Events;

/// <summary>
/// 短信消费者事件
/// </summary>
public class SmsEvent1
{
    public SmsEvent1()
    {
        SmsType = 1;
    }

    public Int32 SmsType { get; set; }

    public Boolean Success { get; set; }
}
