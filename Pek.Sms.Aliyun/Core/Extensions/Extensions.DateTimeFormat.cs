using System.Globalization;

namespace Pek.Sms.Aliyun.Core.Extensions;

public static class DateTimeFormatExtensions
{
    private const String ISO8601_DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";

    public static String ToIso8601DateString(this DateTime dt)
        => dt.ToUniversalTime().ToString(ISO8601_DATE_FORMAT, CultureInfo.CreateSpecificCulture("en-US"));
}