namespace Pek.Sms.Encryption;

internal static class Checker
{
    public static void Data(String data)
    {
        if (String.IsNullOrEmpty(data))
        {
            throw new ArgumentNullException(nameof(data));
        }
    }

    public static void Password(String pwd)
    {
        if (String.IsNullOrEmpty(pwd))
        {
            throw new ArgumentNullException(nameof(pwd));
        }
    }

    public static void IV(String iv)
    {
        if (String.IsNullOrEmpty(iv))
        {
            throw new ArgumentNullException(nameof(iv));
        }
    }

    public static void Key<T>(T key) where T : class
    {
        if (key is String value && String.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(key));
        }

        ArgumentNullException.ThrowIfNull(key);
    }

    public static void Buffer(Byte[] buffer) => ArgumentNullException.ThrowIfNull(buffer);

    public static void Stream(Stream stream) => ArgumentNullException.ThrowIfNull(stream);

    public static void File(String filePath, String? nameOfFilePath = null)
    {
        nameOfFilePath = (String.IsNullOrEmpty(nameOfFilePath) ? "filePath" : nameOfFilePath);
        if (!System.IO.File.Exists(filePath))
        {
            throw new FileNotFoundException(nameOfFilePath);
        }
    }
}