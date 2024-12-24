using System.Text;

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
}
