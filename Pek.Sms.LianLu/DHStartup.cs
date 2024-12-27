using Pek.Ids;
using Pek.Infrastructure;
using Pek.Mail;
using Pek.VirtualFileSystem;

namespace Pek.Sms.LianLu;

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
        var list = SmsSettings.Current.FindByName("fenghuo");
        if (list.Any()) return;

        SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = "fenghuo",
            DisplayName = "烽火",
            SmsType = 0
        });

        SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = "fenghuo",
            DisplayName = "烽火",
            SmsType = 1
        });

        SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = "fenghuo",
            DisplayName = "烽火",
            SmsType = 2
        });

        SmsSettings.Current.Data.Add(new()
        {
            Code = IdHelper.GetIdString(),
            Name = "fenghuo",
            DisplayName = "烽火",
            SmsType = 3
        });
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