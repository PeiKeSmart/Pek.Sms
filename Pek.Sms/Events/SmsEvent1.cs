namespace Pek.Sms.Events;

/// <summary>
/// 短信消费者事件
/// </summary>
public class SmsEvent1
{
    /// <summary>
    /// 实例化一个短信消费者事件
    /// </summary>
    /// <param name="data">参数</param>
    public SmsEvent1(Dictionary<String, Object?>? data)
    {
        SmsType = 1;
        Data = data;
    }

    public Int32 SmsType { get; }

    /// <summary>
    /// 用于确定
    /// </summary>
    public Boolean Success { get; set; }

    /// <summary>
    /// 要处理的数据
    /// </summary>
    public Dictionary<String, Object?>? Data { get; set; }
}
