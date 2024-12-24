using System.Text;

namespace Pek.Sms.Encryption;

internal static class EncodingHelper
{
    public static Encoding Fixed(Encoding? encoding) => encoding ?? Encoding.UTF8;
}