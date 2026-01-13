using NewLife.Serialization;

namespace Pek.Sms.TencentCloud.Models.Results;

/// <summary>腾讯云短信发送结果</summary>
public class TencentSmsResult
{
    /// <summary>是否成功</summary>
    public Boolean IsSuccess { get; set; }

    /// <summary>请求ID</summary>
    public String RequestId { get; set; } = String.Empty;

    /// <summary>错误码</summary>
    public String? Code { get; set; }

    /// <summary>错误信息</summary>
    public String? Message { get; set; }

    /// <summary>发送状态集合</summary>
    public List<SendStatus>? SendStatusSet { get; set; }
}

/// <summary>发送状态</summary>
public class SendStatus
{
    /// <summary>发送流水号</summary>
    public String? SerialNo { get; set; }

    /// <summary>手机号码，E.164标准，+[国家或地区码][手机号]</summary>
    public String? PhoneNumber { get; set; }

    /// <summary>计费条数</summary>
    public Int32 Fee { get; set; }

    /// <summary>用户 session 内容</summary>
    public String? SessionContext { get; set; }

    /// <summary>短信请求错误码</summary>
    public String? Code { get; set; }

    /// <summary>短信请求错误码描述</summary>
    public String? Message { get; set; }

    /// <summary>ISO 国家码</summary>
    public String? IsoCode { get; set; }
}

/// <summary>腾讯云 API 响应</summary>
internal class TencentCloudResponse
{
    /// <summary>响应数据</summary>
    public ResponseData? Response { get; set; }
}

/// <summary>响应数据</summary>
internal class ResponseData
{
    /// <summary>发送状态集合</summary>
    public List<SendStatus>? SendStatusSet { get; set; }

    /// <summary>请求ID</summary>
    public String RequestId { get; set; } = String.Empty;

    /// <summary>错误信息</summary>
    public ErrorInfo? Error { get; set; }
}

/// <summary>错误信息</summary>
internal class ErrorInfo
{
    /// <summary>错误码</summary>
    public String? Code { get; set; }

    /// <summary>错误信息</summary>
    public String? Message { get; set; }
}
