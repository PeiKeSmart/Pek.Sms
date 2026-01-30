namespace Pek.Sms.Events;

/// <summary>
/// 短信消费者事件
/// </summary>
public class SmsEvent3
{
    public SmsEvent3()
    {
        SmsType = 3;
    }

    public Int32 SmsType { get; set; }

    public Boolean Success { get; set; }
}
