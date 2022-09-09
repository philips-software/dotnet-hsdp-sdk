using Microsoft.Extensions.DependencyInjection;

namespace DotnetHsdpSdk.TDR;

public static class ServiceCollectionExtension
{
    public static void AddHsdpTdr(this IServiceCollection serviceCollection, HsdpTdrConfiguration hsdpTdrConfiguration)
    {
        serviceCollection.AddSingleton<IHsdpTdr, HsdpTdr>(p => new HsdpTdr(hsdpTdrConfiguration));
    }
}
