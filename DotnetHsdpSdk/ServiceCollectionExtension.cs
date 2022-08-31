using DotnetHsdpSdk.API;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetHsdpSdk
{
    public static class ServiceCollectionExtension
    {
        public static void AddHsdpIam(this IServiceCollection serviceCollection, HsdpIamConfiguration hsdpIamConfiguration)
        {
            serviceCollection.AddSingleton<IHsdpIam, HsdpIam>(p => new HsdpIam(hsdpIamConfiguration));
        }
    }
}
