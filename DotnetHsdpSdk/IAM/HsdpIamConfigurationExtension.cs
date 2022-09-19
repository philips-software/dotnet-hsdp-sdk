using DotnetHsdpSdk.Utils;
using Microsoft.Extensions.Configuration;

namespace DotnetHsdpSdk.IAM;

public static class HsdpIamConfigurationExtension
{
    public static HsdpIamConfiguration FromAppSettings(this HsdpIamConfiguration @this, IConfiguration configuration,
        string key)
    {
        var iamUrl = configuration[$"{key}:url"];
        var iamClientId = configuration[$"{key}:clientId"];
        var iamClientSecret = configuration[$"{key}:clientSecret"];
        Validate.NotNull(iamUrl, $"{key}:url");
        Validate.NotNull(iamClientId, $"{key}:clientId");
        Validate.NotNull(iamClientSecret, $"{key}:clientSecret");
        return new HsdpIamConfiguration(iamUrl, iamClientId, iamClientSecret);
    }
}
