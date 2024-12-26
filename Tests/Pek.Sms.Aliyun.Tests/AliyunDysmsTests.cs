using NewLife.Log;
using NewLife.Serialization;

using Pek.Mail;
using Pek.Sms.Aliyun.Models;
using Pek.Sms.Aliyun.Models.Results;

namespace Pek.Sms.Aliyun.Tests;

public class AliyunDysmsTests {
    private readonly AliyunDysmsClient _client;
    private readonly SmsData? _config;

    public AliyunDysmsTests()
    {
        _config = SmsSettings.Current.FindByNameAndType("aliyun", 0);

        _client = new AliyunDysmsClient(_config);
    }

    [Fact]
    public async Task CheckResult()
    {
        var client = new Pek.Webs.Clients.WebClient();
        var res = await client.Get("https://www.hlktech.com")
            .IgnoreSsl()
            .Retry(3)
            .WhenCatch<HttpRequestException>(ex =>
            {
                return $"请求失败：{ex.StackTrace}";
            })
            .ResultStringAsync();

        if (res.Contains("请求失败"))
        {
            XTrace.WriteLine($"{res}");
            return;
        }

        XTrace.WriteLine($"获取到的数据：{res}");
    }

    [Fact]
    public async Task CheckResult1()
    {
        var client = new Pek.Webs.Clients.WebClient<AliyunDysmsResult>();
        var res = await client.Get("https://www.deng-hao.com")
            .IgnoreSsl()
            .Retry(3)
            .WhenCatch<HttpRequestException>(ex =>
            {
                return ReturnAsDefautlResponse(ex.Message);
            })
            .ResultFromJsonAsync();

        if (res.Code == "500")
        {
            XTrace.WriteLine($"{res.ToJson()}");
            return;
        }

        XTrace.WriteLine($"获取到的数据：{res.ToJson()}");
    }

    private static AliyunDysmsResult ReturnAsDefautlResponse(String Message)
            => new()
            {
                RequestId = "",
                Code = "500",
                Message = $"解析错误，返回默认结果:{Message}",
                BizId = ""
            };

    [Fact]
    public void ConfigChecking()
    {
        XTrace.WriteLine($"检测数据是否为空：{_config?.ToJson()}");
        Assert.NotNull(_config);
        Assert.NotEmpty(_config.SignName);
        Assert.NotEmpty(_config.AccessKey);
        Assert.NotEmpty(_config.AccessSecret);
    }

    [Fact]
    public async Task SendCodeTest()
    {
        var code = new AliyunDysmsCode
        {
            TemplateCode = "SMS_280805030",
            Phone = ["18307555593"],
            Code = "311920"
        };

        var response = await _client.SendCodeAsync(code);
        XTrace.WriteLine($"收到的数据：{response.ToJson()}");
    }
}