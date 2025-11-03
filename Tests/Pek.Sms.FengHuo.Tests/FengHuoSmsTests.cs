using Pek.Mail;

namespace Pek.Sms.FengHuo.Tests;

public class FengHuoSmsTests
{
    private readonly SmsData? _config;

    public FengHuoSmsTests()
    {
        _config = SmsSettings.Current.FindByNameAndType("fenghuo", 0);
    }
}
