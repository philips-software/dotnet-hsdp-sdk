using DotnetHsdpSdk.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetHsdpSdk.IAM;

public static class ServiceCollectionExtension
{
    public static void AddHsdpIam(this IServiceCollection serviceCollection, HsdpIamConfiguration hsdpIamConfiguration)
    {
        serviceCollection.AddSingleton<IHsdpIam, HsdpIam>(p => new HsdpIam(
            hsdpIamConfiguration, new DateTimeProvider(), new JwtSecurityTokenProvider()
        ));
    }
}
