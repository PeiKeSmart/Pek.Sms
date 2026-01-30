using NewLife.Log;
using NewLife.Serialization;

using Pek.Ids;
using Pek.Infrastructure;
using Pek.VirtualFileSystem;

namespace Pek.Sms.TencentCloud;

/// <summary>
/// 表示应用程序启动时配置SignalR的对象
/// </summary>
public class DHStartup : IDHStartup
{
    /// <summary>
    /// 配置虚拟文件系统
    /// </summary>
    /// <param name="options">虚拟文件配置</param>
    public void ConfigureVirtualFileSystem(DHVirtualFileSystemOptions options)
    {
        options.FileSets.AddEmbedded<DHStartup>(typeof(DHStartup).Namespace);
        // options.FileSets.Add(new EmbeddedFileSet(item.Assembly, item.Namespace));
    }

    /// <summary>
    /// 升级处理逻辑
    /// </summary>
    public void Update()
    {

    }

    /// <summary>
    /// 处理数据
    /// </summary>
    public void ProcessData()
    {
        var list = SmsSettings.Current.FindByName(TencentSmsClient.Name);
        if (list.Any() && list.Count == 4) return;

        if (!list.Any(e => e.SmsType == 0))
            SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = TencentSmsClient.Name,
            DisplayName = "腾讯云",
            SmsType = 0,
            ExtendData = "{\"SdkAppId\": \"1400000000\", \"TemplateId\": \"123456\", \"CountryCode\": \"+86\"}"
        });

        if (!list.Any(e => e.SmsType == 1))
            SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = TencentSmsClient.Name,
            DisplayName = "腾讯云",
            SmsType = 1,
            ExtendData = "{\"SdkAppId\": \"1400000000\", \"TemplateId\": \"123456\", \"CountryCode\": \"+86\"}"
        });

        if (!list.Any(e => e.SmsType == 2))
            SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = TencentSmsClient.Name,
            DisplayName = "腾讯云",
            SmsType = 2,
            ExtendData = "{\"SdkAppId\": \"1400000000\", \"TemplateId\": \"123456\", \"CountryCode\": \"+86\"}"
        });

        if (!list.Any(e => e.SmsType == 3))
            SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = TencentSmsClient.Name,
            DisplayName = "腾讯云",
            SmsType = 3,
            ExtendData = "{\"SdkAppId\": \"1400000000\", \"TemplateId\": \"123456\", \"CountryCode\": \"+86\"}"
        });

        XTrace.WriteLine($"要保存的数据({TencentSmsClient.Name})：{SmsSettings.Current.Data.ToJson()}");

        SmsSettings.Current.Save();
    }

    /// <summary>
    /// 获取此启动配置实现的顺序
    /// </summary>
    public Int32 StartupOrder => 450;

    /// <summary>
    /// 获取此启动配置实现的顺序。主要针对ConfigureMiddleware、UseRouting前执行的数据、UseAuthentication或者UseAuthorization后面 Endpoints前执行的数据
    /// </summary>
    public Int32 ConfigureOrder => 200;
}