namespace Pek.Sms.FengHuo.Models;

/// <summary>
/// 短信上行推送数据
/// </summary>
public class UplinkResult
{
    public Int32 total { get; set; }

    public IList<URResult>? reports { get; set; }
}

public class URResult
{
    public String? user_mobile { get; set; }

    public String? user_content { get; set; }

    public String? channel_num { get; set; }

    public DateTime mo_time { get; set; }
}