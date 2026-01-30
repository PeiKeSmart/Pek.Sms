namespace Pek.Sms.Events;

/// <summary>
/// 短信消费者事件
/// </summary>
public class SmsEvent4
{
    public SmsEvent4()
    {
        SmsType = 4;
    }

    public Int32 SmsType { get; set; }

    public Boolean Success { get; set; }
}
