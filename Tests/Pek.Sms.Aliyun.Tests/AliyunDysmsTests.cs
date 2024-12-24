using System.Text;

using NewLife.Log;

using Pek.Mail.Exceptions;

namespace Pek.Sms.Aliyun.Tests;

public class AliyunDysmsTests {
    private String? _messageIfError { get; set; }

    private readonly AliyunDysmsClient _client;

    public AliyunDysmsTests()
    {
        ExceptionHandleResolver.SetHandler(e => {
            var sb = new StringBuilder();
            sb.AppendLine(e.Message);
            sb.AppendLine(e.Source);
            sb.AppendLine(e.StackTrace);
            _messageIfError += sb.ToString();
        });

        


        _client = new AliyunDysmsClient(ExceptionHandleResolver.ResolveHandler());
    }

    [Fact]
    public async Task CheckResult()
    {
        var client = new Pek.Webs.Clients.WebClient();
        var res = await client.Get("https://www.deng-hao.com")
            .Retry(3)
            .WhenCatch<HttpRequestException>(ex =>
            {
                XTrace.WriteLine($"请求失败：{ex.Message}");
                return $"请求失败：{ex.Message}";
            })
            .ResultAsync();

        XTrace.WriteLine($"获取到的数据：{res}");
    }
}
