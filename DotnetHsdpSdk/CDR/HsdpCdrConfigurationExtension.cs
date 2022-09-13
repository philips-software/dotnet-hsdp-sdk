using System;
using DotnetHsdpSdk.Utils;
using Microsoft.Extensions.Configuration;

namespace DotnetHsdpSdk.CDR;

public static class HsdpCdrConfigurationExtension
{
    public static HsdpCdrConfiguration FromAppSettings(this HsdpCdrConfiguration @this, IConfiguration configuration,
        string key)
    {
        var cdrUrl = configuration.GetMandatoryKey(key, "url");
        var fhirVersion = configuration.GetMandatoryKey(key, "fhirVersion");
        var mediaType = configuration.GetMandatoryKey(key, "mediaType");
        return new HsdpCdrConfiguration(new Uri(cdrUrl), fhirVersion, mediaType);
    }
}
