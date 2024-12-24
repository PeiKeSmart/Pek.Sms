using System.Security.Cryptography;
using System.Text;

namespace Pek.Sms.Encryption;

public abstract class HMACHashingBase
{
    protected static String Encrypt<T>(String data, String key, Encoding? encoding = null) where T : KeyedHashAlgorithm, new()
    {
        Checker.Data(data);
        Checker.Key(key);
        encoding = EncodingHelper.Fixed(encoding);
        using KeyedHashAlgorithm keyedHashAlgorithm = new T();
        keyedHashAlgorithm.Key = encoding.GetBytes(key);
        return keyedHashAlgorithm.ComputeHash(encoding.GetBytes(data)).ToHexString();
    }
}