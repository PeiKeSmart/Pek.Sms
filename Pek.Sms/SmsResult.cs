namespace Pek.Sms;

/// <summary>
/// 通用短信接口返回结果
/// </summary>
public class SmsResult
{
    /// <summary>
    /// 初始化短信接口返回结果
    /// </summary>
    /// <param name="success">是否发送成功</param>
    /// <param name="raw">短信提供商返回的原始消息</param>
    /// <param name="Message">信息</param>
    public SmsResult(Boolean success = true, String Message = "", String raw = "")
    {
        Success = success;
        Raw = raw;
    }

    /// <summary>
    /// 是否发送成功
    /// </summary>
    public Boolean Success { get; }

    /// <summary>
    /// 短信提供商返回的原始消息
    /// </summary>
    public String Raw { get; }

    /// <summary>
    /// 消息
    /// </summary>
    public String Message { get; }

    /// <summary>
    /// 成功消息
    /// </summary>
    public static SmsResult Ok { get; } = new SmsResult();
}
