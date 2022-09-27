using DotnetHsdpSdk.Utils;
using Microsoft.Extensions.Configuration;

namespace DotnetHsdpSdk.CDR;

public static class HsdpCdrConfigurationExtension
{
    public static HsdpCdrConfiguration FromAppSettings(IConfiguration configuration, string key)
    {
        var cdrUrl = configuration.GetMandatoryKey(key, "url");
        var fhirVersion = configuration.GetMandatoryKey(key, "fhirVersion");
        var mediaType = configuration.GetMandatoryKey(key, "mediaType");
        return new HsdpCdrConfiguration(cdrUrl, fhirVersion, mediaType);
    }
}
