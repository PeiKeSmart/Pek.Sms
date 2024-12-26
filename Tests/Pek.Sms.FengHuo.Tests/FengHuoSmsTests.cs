using Pek.Mail;

namespace Pek.Sms.FengHuo.Tests;

public class FengHuoSmsTests
{
    private readonly SmsData? _config;

    public FengHuoSmsTests()
    {
        _config = SmsSettings.Current.FindByNameAndType("aliyun", 0);
    }
}
