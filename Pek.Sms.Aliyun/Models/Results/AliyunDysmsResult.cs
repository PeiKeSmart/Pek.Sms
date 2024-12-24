namespace Pek.Sms.Aliyun.Models.Results;

public class AliyunDysmsResult
{
    public String? Recommend { get; set; }

    public String? RequestId { get; set; }

    public String? Code { get; set; }

    public String? Message { get; set; }

    public String? BizId { get; set; }

    public Boolean IsSuccess() => String.Compare(Code, "ok", StringComparison.OrdinalIgnoreCase) == 0;
}
