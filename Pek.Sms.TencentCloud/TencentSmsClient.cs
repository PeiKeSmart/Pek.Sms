using NewLife.Configuration;

using Pek.Mail;
using Pek.Mail.Client;

namespace Pek.Sms.TencentCloud;

/// <summary>
/// Tencent Cloud SMS / QCloud SMS
/// </summary>
/// <remarks>Need to refactor</remarks>
public class TencentSmsClient : SmsClientBase
{
    private readonly SmsData _config;

    public override void CheckMyself() { }

    public TencentSmsClient(SmsData? config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }
}
