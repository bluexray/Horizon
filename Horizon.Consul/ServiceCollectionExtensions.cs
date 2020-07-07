using Horizon.Consul.Configurations;
using Horizon.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Horizon.Consul
{
    public static class ServiceCollectionExtensions
    {




        public static IServiceCollection AddHorizonConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulServiceDiscoveryOption>(configuration.GetSection("ServiceDiscovery"));

            services.AddSingleton<IRegistryHost, ConsulProxy>(); 

            return services;

            //configuration.GetSection("ServiceDiscovery: Consul");
           
        }
    } 
}
