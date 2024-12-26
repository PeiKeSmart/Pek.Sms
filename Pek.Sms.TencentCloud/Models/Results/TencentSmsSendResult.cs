namespace Pek.Sms.TencentCloud.Models.Results;

public class TencentSmsSendResult
{
    public Int32 Result { get; set; }
    public String? ErrMsg { get; set; }
    public String? Sid { get; set; }
    public Int32 Fee { get; set; }
    public String? Mobile { get; set; }
    public String? NationCode { get; set; }
}

public class TencentSmsSendResponseData
{
    private Int32 FeeInternal { get; set; }
    private String? SidInternal { get; set; }

    public Int32 Result { get; set; }
    public String? ErrMsg { get; set; }
    public String? Ext { get; set; }

    public Int32 Fee
    {
        get
        {
            if (Detail == null || Detail.Count == 0) return FeeInternal;
            return Detail.Sum(x => x.Fee);
        }
        set
        {
            if (Detail == null || Detail.Count == 0) FeeInternal = value;
        }
    }

    public String? Sid
    {
        get
        {
            if (Detail == null || Detail.Count == 0) return SidInternal;
            return Detail.FirstOrDefault()?.Sid;
        }
        set
        {
            if (Detail == null || Detail.Count == 0) SidInternal = value;
        }
    }

    public List<TencentSmsSendResult>? Detail { get; set; }
}