namespace Pek.Sms.Exceptions;

public class InvalidArgumentException(String message, String serviceName, Int32 errorCode) : Exception(message), ISmsException
{
    public String ServiceName { get; } = serviceName;

    public Int32 ErrorCode { get; } = errorCode;

    public override String ToString() => $"code:{ErrorCode},message:{Message}";
}