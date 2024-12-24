namespace Pek.Sms.Aliyun.Core.Extensions;

public static class StringExtensions
{
    public static Byte[] HexToBytes(this String value)
    {
        var index = 0;
        var list = new List<Byte>(value.Length / 2);

        while (index < value.Length)
        {
            list.Add(Byte.Parse(value.Substring(index, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            index += 2;
        }

        return [.. list];
    }
}