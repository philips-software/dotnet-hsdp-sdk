using System;

namespace DotnetHsdpSdk.Utils;

public static class Base64ExtensionMethods
{
    public static string EncodeBase64(this byte[] value)
    {
        return Convert.ToBase64String(value);
    }

    public static byte[] DecodeBase64(this string value)
    {
        return Convert.FromBase64String(value);
    }
}
