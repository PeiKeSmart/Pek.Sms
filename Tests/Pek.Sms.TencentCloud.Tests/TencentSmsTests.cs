using Pek.Mail;

namespace Pek.Sms.TencentCloud.Tests;

public class TencentSmsTests {
    private readonly SmsData? _config;
    private readonly TencentSmsClient _client;

    public TencentSmsTests()
    {
        _config = SmsSettings.Current.FindByNameAndType("tencent", 0);
        _client = new TencentSmsClient(_config);
    }
}
