using Microsoft.Extensions.DependencyInjection;

namespace DotnetHsdpSdk.CDR;

public static class ServiceCollectionExtension
{
    public static void AddHsdpCdr(this IServiceCollection serviceCollection, HsdpCdrConfiguration hsdpCdrConfiguration)
    {
        serviceCollection.AddSingleton<IHsdpCdr, HsdpCdr>(p => new HsdpCdr(hsdpCdrConfiguration));
    }
}
