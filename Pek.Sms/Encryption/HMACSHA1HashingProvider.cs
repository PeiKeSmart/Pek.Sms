using System.Security.Cryptography;
using System.Text;

namespace Pek.Sms.Encryption;

public sealed class HMACSHA1HashingProvider : HMACHashingBase
{
    private HMACSHA1HashingProvider()
    {
    }

    public static String Signature(String data, String key, Encoding? encoding = null) => Encrypt<HMACSHA1>(data, key, encoding);

    public static Boolean Verify(String comparison, String data, String key, Encoding? encoding = null) => comparison == Signature(data, key, encoding);
}