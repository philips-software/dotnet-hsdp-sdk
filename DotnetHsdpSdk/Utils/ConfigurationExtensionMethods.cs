using Microsoft.Extensions.Configuration;

namespace DotnetHsdpSdk.Utils;

public static class ConfigurationExtensionMethods
{
    public static string GetMandatoryKey(this IConfiguration configuration, string key, string relativePath)
    {
        var absolutePath = $"{key}:{relativePath}";
        var field = configuration[absolutePath];
        Validate.NotNull(field, absolutePath);
        return field!;
    }
}
