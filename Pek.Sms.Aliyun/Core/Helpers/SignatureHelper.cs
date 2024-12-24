using System.Globalization;
using System.Text;

using Pek.Sms.Aliyun.Core.Extensions;

namespace Pek.Sms.Aliyun.Core.Helpers;

/// <summary>
/// Signature helper
/// documentation:
///     https://help.aliyun.com/document_detail/30079.html?spm=5176.7739992.2.3.HM7WTG
/// reference to:
///     https://github.com/yaosansi/aliyun-openapi-sdk-lite/blob/master/SignatureHelper.cs
/// </summary>
public static class SignatureHelper
{
    // ReSharper disable once InconsistentNaming
    private const String PERCENT_ENCODE_TEXT = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

    public static String GetApiSignature(IDictionary<String, String> coll, String key)
    {
        var origin = coll.GetOrigin();

        var sign = origin.Signature(key);

        // hex string to byte array
        var buffer = sign.HexToBytes();

        // convert bytes to base64 string and return
        return Convert.ToBase64String(buffer);
    }

    private static String GetOrigin(this IDictionary<String, String> coll) => $"POST&%2F&{PercentEncode(String.Join("&", coll.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => $"{PercentEncode(x.Key)}={PercentEncode(x.Value)}")))}";

    private static string Signature(this string orgin, string key)
    {
        return HMACSHA1HashingProvider.Signature(orgin, $"{key}&", Encoding.UTF8);
    }

    private static string PercentEncode(string value)
    {
        var stringBuilder = new StringBuilder();

        var bytes = Encoding.UTF8.GetBytes(value);

        foreach (var b in bytes)
        {
            var c = (char)b;

            if (PERCENT_ENCODE_TEXT.IndexOf(c) >= 0)
            {
                stringBuilder.Append(c);
            }
            else
            {
                stringBuilder
                    .Append("%")
                    .Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
            }
        }

        return stringBuilder.ToString();
    }
}