namespace DotnetHsdpSdk.Utils;

public static class HttpStatusCodeExtensionMethods
{
    public static bool IsSuccess(this int statusCode)
    {
        return statusCode is >= 200 and < 300;
    }
}
