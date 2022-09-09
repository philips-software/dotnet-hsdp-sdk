using System;
using DotnetHsdpSdk.Utils;
using Microsoft.Extensions.Configuration;

namespace DotnetHsdpSdk.TDR;

public static class HsdpTdrConfigurationExtension
{
    public static HsdpTdrConfiguration FromAppSettings(this HsdpTdrConfiguration @this, IConfiguration configuration,
        string key)
    {
        var tdrUrl = configuration[$"{key}:url"];
        Validate.NotNull(tdrUrl, $"{key}:url");
        return new HsdpTdrConfiguration(new Uri(tdrUrl));
    }
}
