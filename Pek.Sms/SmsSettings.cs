using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using NewLife;
using NewLife.Configuration;
using NewLife.Serialization;

namespace Pek.Sms;

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

    ///// <summary>加载时触发</summary>
    //protected override void OnLoaded()
    //{
    //    if (Data == null || Data.Count == 0)
    //    {
    //        var list = new List<SmsData>
    //        {
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "fenghuo",
    //                DisplayName = "烽火",
    //                SmsType = 0
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "fenghuo",
    //                DisplayName = "烽火",
    //                SmsType = 1
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "fenghuo",
    //                DisplayName = "烽火",
    //                SmsType = 2
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "fenghuo",
    //                DisplayName = "烽火",
    //                SmsType = 3
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "lianlu",
    //                DisplayName = "联麓",
    //                SmsType = 0
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "lianlu",
    //                DisplayName = "联麓",
    //                SmsType = 1
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "lianlu",
    //                DisplayName = "联麓",
    //                SmsType = 2
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "lianlu",
    //                DisplayName = "联麓",
    //                SmsType = 3
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "aliyun",
    //                DisplayName = "阿里云",
    //                SmsType = 0,
    //                //ExtendFields = "RetryTimes",
    //                //ExtendData = "{\"RetryTimes\": \"3\"}"
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "aliyun",
    //                DisplayName = "阿里云",
    //                SmsType = 1,
    //                //ExtendFields = "RetryTimes",
    //                //ExtendData = "{\"RetryTimes\": \"3\"}"
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "aliyun",
    //                DisplayName = "阿里云",
    //                SmsType = 2,
    //                //ExtendFields = "RetryTimes",
    //                //ExtendData = "{\"RetryTimes\": \"3\"}"
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "aliyun",
    //                DisplayName = "阿里云",
    //                SmsType = 3,
    //                //ExtendFields = "RetryTimes",
    //                //ExtendData = "{\"RetryTimes\": \"3\"}"
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "tencent",
    //                DisplayName = "腾讯云",
    //                SmsType = 0
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "tencent",
    //                DisplayName = "腾讯云",
    //                SmsType = 1
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "tencent",
    //                DisplayName = "腾讯云",
    //                SmsType = 2
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "tencent",
    //                DisplayName = "腾讯云",
    //                SmsType = 3
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "mysubmail",
    //                DisplayName = "赛邮云",
    //                SmsType = 0
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "mysubmail",
    //                DisplayName = "赛邮云",
    //                SmsType = 1
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "mysubmail",
    //                DisplayName = "赛邮云",
    //                SmsType = 2
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "mysubmail",
    //                DisplayName = "赛邮云",
    //                SmsType = 3
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "netease",
    //                DisplayName = "网易云信",
    //                SmsType = 0
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "netease",
    //                DisplayName = "网易云信",
    //                SmsType = 1
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "netease",
    //                DisplayName = "网易云信",
    //                SmsType = 2
    //            },
    //            new() {
    //                Code = IdHelper.GetIdString(),
    //                Name = "netease",
    //                DisplayName = "网易云信",
    //                SmsType = 3
    //            }
    //        };

    //        Data = list;
    //    }

    //    base.OnLoaded();
    //}

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

    /// <summary>根据名称和类型获取数据</summary>
    public SmsData? FindByNameAndType(String Name, Int32 SmsType)
    {
        foreach (var item in Data)
        {
            if (item.Name.EqualIgnoreCase(Name) && item.SmsType == SmsType) return item;
        }
        return null;
    }

    public IList<SmsData> FindByName(String Name) => Data.FindAll(e => e.Name.EqualIgnoreCase(Name));
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
    public String AccessKey { get; set; } = String.Empty;

    /// <summary>
    /// AccessKeySecret/AppKey
    /// </summary>
    [Description("AccessKeySecret/AppKey")]
    public String AccessSecret { get; set; } = String.Empty;

    /// <summary>
    /// 签名
    /// </summary>
    [Description("签名")]
    public String SignName { get; set; } = String.Empty;

    /// <summary>
    /// 排序。同一类型按照此字段排序执行，值越小优先级越高，当上一执行失败时由后面的重新发送。
    /// </summary>
    [Description("排序")]
    public Int32 Order { get; set; }

    /// <summary>
    /// 请求超时时间
    /// </summary>
    [Description("请求超时时间")]
    public Int32 Timeout { get; set; } = 60000;

    /// <summary>
    /// 是否请求https
    /// </summary>
    [Description("是否请求https")]
    public Boolean Security { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    [Description("重试次数")]
    public Int32 RetryTimes { get; set; } = 3;

    /// <summary>
    /// 是否启用代理。启用后本地调试可通过代理访问限制IP的短信平台
    /// </summary>
    [Description("是否启用代理")]
    public Boolean EnableProxy { get; set; }

    /// <summary>
    /// 代理服务器地址。例如：https://proxy.0ht.cn
    /// </summary>
    [Description("代理服务器地址")]
    public String? ProxyUrl { get; set; }

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

    /// <summary>
    /// 扩展内容。转为字典
    /// </summary>
    [Description("扩展内容。转为字典")]
    [XmlIgnore, IgnoreDataMember]
    public IDictionary<String, Object?> Data
    {
        get
        {
            if (ExtendData.IsNullOrWhiteSpace()) return new Dictionary<String, Object?>();

            return JsonHelper.DecodeJson(ExtendData) ?? new Dictionary<String, Object?>();
        }
    }
}