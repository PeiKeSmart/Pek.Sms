using Pek.Mail;
using Xunit;

namespace Pek.Sms.FengHuo.Tests;

public class FengHuoSmsTests
{
    private readonly SmsData? _config;

    public FengHuoSmsTests()
    {
        _config = SmsSettings.Current.FindByNameAndType("fenghuo", 0);
    }

    [Fact(DisplayName = "测试短信客户端初始化")]
    public void TestClientInitialization()
    {
        if (_config == null)
        {
            // 配置不存在时跳过测试
            return;
        }

        var client = new FengHuoSmsClient(_config);
        Assert.NotNull(client);
    }

    [Fact(DisplayName = "测试发送普通短信（需配置）")]
    public async Task TestSendAsync()
    {
        if (_config == null || !_config.IsEnabled)
        {
            // 配置不存在或未启用时跳过测试
            return;
        }

        var client = new FengHuoSmsClient(_config);
        
        // 注意：实际测试需要真实的手机号和已报备的签名
        // var result = await client.SendAsync("13800138000", "【测试签名】您的验证码是123456");
        // Assert.True(result.Success);
    }

    [Fact(DisplayName = "测试发送模板短信（需配置）")]
    public async Task TestSendTemplateAsync()
    {
        if (_config == null || !_config.IsEnabled)
        {
            // 配置不存在或未启用时跳过测试
            return;
        }

        var client = new FengHuoSmsClient(_config);
        
        // 注意：实际测试需要真实的手机号和模板ID
        // var result = await client.SendTemplateParamd("13800138000", "123", ["123456"], null);
        // Assert.True(result.Success);
    }
}

