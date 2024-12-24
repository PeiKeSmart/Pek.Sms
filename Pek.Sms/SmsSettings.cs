using System.ComponentModel;

using NewLife.Configuration;

using Pek.Ids;

namespace Pek.Mail;

/// <summary>短信设置</summary>
[DisplayName("短信设置")]
[Config("Sms")]
public class SmsSettings : Config<SmsSettings>
{
    /// <summary>
    /// 短信数据
    /// </summary>
    [Description("短信数据")]
    public List<SmsData> Data { get; set; } = [];

    /// <summary>实例化</summary>
    public SmsSettings() { }

    /// <summary>加载时触发</summary>
    protected override void OnLoaded()
    {
        if (Data == null || Data.Count == 0)
        {
            var list = new List<SmsData>
            {
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "fenghuo",
                    DisplayName = "烽火",
                    SmsType = 0
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "fenghuo",
                    DisplayName = "烽火",
                    SmsType = 1
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "fenghuo",
                    DisplayName = "烽火",
                    SmsType = 2
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "fenghuo",
                    DisplayName = "烽火",
                    SmsType = 3
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "lianlu",
                    DisplayName = "联麓",
                    SmsType = 0
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "lianlu",
                    DisplayName = "联麓",
                    SmsType = 1
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "lianlu",
                    DisplayName = "联麓",
                    SmsType = 2
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "lianlu",
                    DisplayName = "联麓",
                    SmsType = 3
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "aliyun",
                    DisplayName = "阿里云",
                    SmsType = 0,
                    ExtendFields = "RetryTimes",
                    ExtendData = "{\"RetryTimes\": \"3\"}"
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "aliyun",
                    DisplayName = "阿里云",
                    SmsType = 1
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "aliyun",
                    DisplayName = "阿里云",
                    SmsType = 2
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "aliyun",
                    DisplayName = "阿里云",
                    SmsType = 3
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "tencent",
                    DisplayName = "腾讯云",
                    SmsType = 0
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "tencent",
                    DisplayName = "腾讯云",
                    SmsType = 1
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "tencent",
                    DisplayName = "腾讯云",
                    SmsType = 2
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "tencent",
                    DisplayName = "腾讯云",
                    SmsType = 3
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "mysubmail",
                    DisplayName = "赛邮云",
                    SmsType = 0
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "mysubmail",
                    DisplayName = "赛邮云",
                    SmsType = 1
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "mysubmail",
                    DisplayName = "赛邮云",
                    SmsType = 2
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "mysubmail",
                    DisplayName = "赛邮云",
                    SmsType = 3
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "netease",
                    DisplayName = "网易云信",
                    SmsType = 0
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "netease",
                    DisplayName = "网易云信",
                    SmsType = 1
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "netease",
                    DisplayName = "网易云信",
                    SmsType = 2
                },
                new() {
                    Code = IdHelper.GetIdString(),
                    Name = "netease",
                    DisplayName = "网易云信",
                    SmsType = 3
                }
            };

            Data = list;
        }

        base.OnLoaded();
    }

    /// <summary>获取默认的配置数据</summary>
    public SmsData FindDefault(Int32 SmsType) => Data.FirstOrDefault(e => e.SmsType == SmsType && e.IsDefault) ?? Data[0];

    /// <summary>根据惟一标识获取数据</summary>
    public SmsData? FindByCode(String Code)
    {
        foreach (var item in Data)
        {
            if (item.Code == Code) return item;
        }
        return null;
    }
}

/// <summary>
/// 短信数据
/// </summary>
public class SmsData
{
    /// <summary>
    /// 惟一标识
    /// </summary>
    [Description("惟一标识")]
    public String? Code { get; set; }

    /// <summary>
    /// 服务商标识
    /// </summary>
    [Description("服务商标识")]
    public String? Name { get; set; }

    /// <summary>
    /// 显示名称
    /// </summary>
    [Description("显示名称")]
    public String? DisplayName { get; set; }

    /// <summary>
    /// 短信类型。0为国内通知类，1为国际通知类，2为国内营销类，3为国际营销类
    /// </summary>
    [Description("短信类型。0为国内通知类，1为国际通知类，2为国内营销类，3为国际营销类")]
    public Int32 SmsType { get; set; }

    /// <summary>
    /// 与短信类型搭配使用，同一类型只能有一个默认
    /// </summary>
    [Description("与短信类型搭配使用，同一类型只能有一个默认")]
    public Boolean IsDefault { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    [Description("是否启用")]
    public Boolean IsEnabled { get; set; }

    /// <summary>
    /// AccessKey/AccessKeyId/AppId
    /// </summary>
    [Description("AccessKey/AccessKeyId/AppId")]
    public String? AccessKey { get; set; }

    /// <summary>
    /// AccessKeySecret/AppKey
    /// </summary>
    [Description("AccessKeySecret/AppKey")]
    public String? AccessSecret { get; set; }

    /// <summary>
    /// 签名
    /// </summary>
    [Description("签名")]
    public String? PassKey { get; set; }

    /// <summary>
    /// 扩展字段
    /// </summary>
    [Description("扩展字段")]
    public String? ExtendFields { get; set; }

    /// <summary>
    /// 扩展内容
    /// </summary>
    [Description("扩展内容")]
    public String? ExtendData { get; set; }
}