using Consul;
using DnsClient;
using Horizon.Consul.Configurations;
using Horizon.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Runtime.CompilerServices;

namespace Horizon.Consul
{
    public static class ServiceCollectionExtensions
    {


        public static IServiceCollection AddHorizonConsul(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<ConsulServiceDiscoveryOption>(configuration.GetSection("ServiceDiscovery"));

            services.AddSingleton<IRegistryHost, ConsulProxy>();

            //Console.WriteLine("consulproxy .....regsited");


            services.RegisterDnsLookup();

            ConsulServiceDiscoveryOption serviceDiscoveryOption = new ConsulServiceDiscoveryOption();
            configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);

            services.AddConsul(() => new ConsulProxy(serviceDiscoveryOption.Consul));

            return services;
            //configuration.GetSection("ServiceDiscovery: Consul");
           
        }


        public static IServiceCollection AddConsul(this IServiceCollection services, Func<IRegistryHost> registryHostFactory)
        {
            var registryHost = registryHostFactory();
            var serviceRegistry = new ServiceRegistry(registryHost);
            services.AddSingleton(serviceRegistry);
           
            //services.AddTransient<IStartupFilter, NanoStartupFilter>();
            return services;
        }

       
        private static IServiceCollection RegisterDnsLookup(this IServiceCollection services)
        {
            //implement the dns lookup and register to service container
            services.TryAddSingleton<IDnsQuery>(p =>
            {
                var serviceConfiguration = p.GetRequiredService<IOptions<ConsulServiceDiscoveryOption>>().Value;

                //LookupClientOptions ops = new LookupClientOptions()
                //{
                //    EnableAuditTrail = false,
                //    UseCache = true,
                //    MinimumCacheTimeout = TimeSpan.FromSeconds(1),

                //};

                //var client = new LookupClient(ops);

                var client = new LookupClient(IPAddress.Parse("127.0.0.1"), 8600);

                

                if (serviceConfiguration.Consul.DnsEndpoint != null)
                {
                    client = new LookupClient(serviceConfiguration.Consul.DnsEndpoint.ToIPEndPoint());
                }


                client.EnableAuditTrail = false;
                client.UseCache = true;
                client.MinimumCacheTimeout = TimeSpan.FromSeconds(1);

                return client;
            });
            return services;
        }
    } 
}