using Pek.Mail.Client;

namespace Pek.Sms.Aliyun;

public class AliyunDysmsClient : SmsClientBase
{
    public override void CheckMyself() { }

    public AliyunDysmsClient(Action<Exception>? exceptionHandler = null)
    {

    }
}
