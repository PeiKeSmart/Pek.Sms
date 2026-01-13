using System.Security.Cryptography;
using System.Text;

using Pek.Timing;

namespace Pek.Sms.TencentCloud.Core;

/// <summary>腾讯云签名助手</summary>
internal static class TencentSignatureHelper
{
    /// <summary>生成 TC3-HMAC-SHA256 签名</summary>
    /// <param name="secretId">密钥ID</param>
    /// <param name="secretKey">密钥</param>
    /// <param name="payload">请求体JSON字符串</param>
    /// <param name="timestamp">时间戳（秒）</param>
    /// <returns>Authorization 请求头值</returns>
    public static String GenerateAuthorization(String secretId, String secretKey, String payload, Int64 timestamp)
    {
        const String host = "sms.tencentcloudapi.com";
        const String service = "sms";
        const String algorithm = "TC3-HMAC-SHA256";
        const String httpRequestMethod = "POST";
        const String canonicalUri = "/";
        const String canonicalQueryString = "";

        // ************* 步骤 1：拼接规范请求串 *************
        var hashedRequestPayload = ComputeSha256(payload);
        var canonicalHeaders = $"content-type:application/json; charset=utf-8\nhost:{host}\n";
        var signedHeaders = "content-type;host";
        var canonicalRequest = $"{httpRequestMethod}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{hashedRequestPayload}";

        // ************* 步骤 2：拼接待签名字符串 *************
        var dateStr = UnixTime.ToDateTime(timestamp).ToString("yyyy-MM-dd");
        var credentialScope = $"{dateStr}/{service}/tc3_request";
        var hashedCanonicalRequest = ComputeSha256(canonicalRequest);
        var stringToSign = $"{algorithm}\n{timestamp}\n{credentialScope}\n{hashedCanonicalRequest}";

        // ************* 步骤 3：计算签名 *************
        var secretDate = HmacSha256(Encoding.UTF8.GetBytes($"TC3{secretKey}"), Encoding.UTF8.GetBytes(dateStr));
        var secretService = HmacSha256(secretDate, Encoding.UTF8.GetBytes(service));
        var secretSigning = HmacSha256(secretService, Encoding.UTF8.GetBytes("tc3_request"));
        var signature = HmacSha256(secretSigning, Encoding.UTF8.GetBytes(stringToSign));
        var signatureHex = BitConverter.ToString(signature).Replace("-", "").ToLower();

        // ************* 步骤 4：拼接 Authorization *************
        return $"{algorithm} Credential={secretId}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signatureHex}";
    }

    /// <summary>计算 SHA256 哈希</summary>
    private static String ComputeSha256(String input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    /// <summary>计算 HMAC-SHA256</summary>
    private static Byte[] HmacSha256(Byte[] key, Byte[] message)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(message);
    }
}
