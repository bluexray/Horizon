using Horizon.Consul.Configurations;
using Horizon.Core.Model;
using Horizon.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Horizon.Core.Docker;
using Horizon.Core.Extensions;

namespace Horizon.Consul
{
    public static class ApplicationBuilderExtensions
    {
        

        public static IApplicationBuilder UseHorizonConsul(this IApplicationBuilder app, IConfiguration configuration)
        {

            ConsulServiceDiscoveryOption serviceDiscoveryOption = new ConsulServiceDiscoveryOption();
            configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);
            app.UseConsulRegisterService(serviceDiscoveryOption);
            return app;

        }

        public static IApplicationBuilder UseConsulRegisterService(this IApplicationBuilder app, ConsulServiceDiscoveryOption serviceDiscoveryOption)
        {
            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>() ??
                         throw new ArgumentException("Missing Dependency", nameof(IApplicationLifetime));
            if (serviceDiscoveryOption.Consul == null)
                throw new ArgumentException("Missing Dependency", nameof(serviceDiscoveryOption.Consul));



            if (string.IsNullOrEmpty(serviceDiscoveryOption.ServiceName))
                throw new ArgumentException("service name must be configure", nameof(serviceDiscoveryOption.ServiceName));

            IEnumerable<Uri> addresses = null;

            var dockerip = "";

            if (serviceDiscoveryOption.Endpoints != null && serviceDiscoveryOption.Endpoints.Length > 0)
            {
                Log.Information($"Using {serviceDiscoveryOption.Endpoints.Length} configured endpoints for service registration");
                addresses = serviceDiscoveryOption.Endpoints.Select(p => new Uri(p));
            }
            else
            {
                if (!DockerHelper.IsRunningInDocker)
                {
                    Log.Information($"Trying to use server.Features to figure out the service endpoint for registration.");
                    var features = app.Properties["server.Features"] as FeatureCollection;
                    addresses = features.Get<IServerAddressesFeature>().Addresses.Select(p => new Uri(p)).ToArray();
                    Log.Information($"this services ip is : {addresses.First().DnsSafeHost}");
                }
                else
                {
                    Log.Information($"Triying to use docker to figure out the service endpoint for registration.");

                    Log.Information($"The Docker IP is {DockerHelper.ContainerAddress}....setup 1.");
                    if (DockerHelper.ContainerAddress != null)
                    {
                        Log.Information($"The Docker IP is {DockerHelper.ContainerAddress}....setup 2.");
                        dockerip = DockerHelper.ContainerAddress;
                        Log.Information($"This services {DockerHelper.ContainerAddress} on docker.");
                    }
                    else
                    {
                        throw new ArgumentException("can not get docker services ip");
                    }
                }
            }

            //run in docker
            if (DockerHelper.IsRunningInDocker)
            {
                var port=5022;
                var address = new Uri("http://" + dockerip + ":" + port);
                var serviceID = GetServiceId(serviceDiscoveryOption.ServiceName, address);
                Log.Information($"Registering service {serviceID} for address {dockerip}.");
                Uri healthCheck = null;
                string url = "";
                if (!string.IsNullOrEmpty(serviceDiscoveryOption.HealthCheckTemplate))
                {
                    url = "http://" + dockerip + ":" + serviceDiscoveryOption.HealthCheckTemplate;


                    healthCheck = new Uri(url);
                    //healthCheck = new Uri(address, serviceDiscoveryOption.HealthCheckTemplate);
                    Log.Information($"Adding healthcheck for {serviceID},checking {healthCheck}");
                }
                var registryInformation = app.AddTenant(serviceDiscoveryOption.ServiceName, serviceDiscoveryOption.Version, address, healthCheckUri: healthCheck, tags: new[] { $"urlprefix-/{serviceDiscoveryOption.ServiceName}" });
                Log.Information($"Registering sevices {serviceDiscoveryOption.ServiceName} and {healthCheck} on {address}...........");

                // register service & health check cleanup
                applicationLifetime.ApplicationStopping.Register(() =>
                {
                    Log.Information("Removing tenant & additional health check");
                    app.RemoveTenant(registryInformation.Id);
                });
                return app;
            }



            //run in os
            foreach (var address in addresses)
            {
                var serviceID = GetServiceId(serviceDiscoveryOption.ServiceName, address);
                Log.Information($"Registering service {serviceID} for address {address}.");
                Uri healthCheck = null;
                string url = "";
                if (!string.IsNullOrEmpty(serviceDiscoveryOption.HealthCheckTemplate))
                {
                    url = "http://" + address.DnsSafeHost + ":" + serviceDiscoveryOption.HealthCheckTemplate;


                    healthCheck = new Uri(url);
                    //healthCheck = new Uri(address, serviceDiscoveryOption.HealthCheckTemplate);
                    Log.Information($"Adding healthcheck for {serviceID},checking {healthCheck}");
                }
                var registryInformation = app.AddTenant(serviceDiscoveryOption.ServiceName, serviceDiscoveryOption.Version, address, healthCheckUri: healthCheck, tags: new[] { $"urlprefix-/{serviceDiscoveryOption.ServiceName}" });
                Log.Information($"Registering sevices {serviceDiscoveryOption.ServiceName} and {healthCheck} on {address}...........");

                // register service & health check cleanup
                applicationLifetime.ApplicationStopping.Register(() =>
                {
                    Log.Information("Removing tenant & additional health check");
                    app.RemoveTenant(registryInformation.Id);
                });
            }
            return app;
        }

        private static String GetServiceId(string serviceName, Uri address)
        {
            return $"{serviceName}_{address.Host.Replace(".", "_")}_{address.Port}";
        }


        public static ServiceInformation AddTenant(this IApplicationBuilder app, string serviceName, string version, Uri uri, Uri healthCheckUri = null, IEnumerable<string> tags = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<ServiceRegistry>();


            Log.Information($"Registering sevices {serviceName} and {healthCheckUri} on {uri}...........");

            var serviceInformation = serviceRegistry.RegisterServiceAsync(serviceName, version, uri, healthCheckUri, tags)
                .Result;



            return serviceInformation;
        }


        public static bool RemoveTenant(this IApplicationBuilder app, string serviceId)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (string.IsNullOrEmpty(serviceId))
            {
                throw new ArgumentNullException(nameof(serviceId));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<ServiceRegistry>();
            return serviceRegistry.DeregisterServiceAsync(serviceId)
                .Result;
        }


        public static string AddHealthCheck(this IApplicationBuilder app, ServiceInformation registryInformation, Uri checkUri, TimeSpan? interval = null, string notes = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (registryInformation == null)
            {
                throw new ArgumentNullException(nameof(registryInformation));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<ServiceRegistry>();
            string checkId = serviceRegistry.RegisterHealthCheckAsync(registryInformation.Name, registryInformation.Id, checkUri, interval, notes)
                .Result;

            return checkId;
        }

        public static bool RemoveHealthCheck(this IApplicationBuilder app, string checkId)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (string.IsNullOrEmpty(checkId))
            {
                throw new ArgumentNullException(nameof(checkId));
            }

            var serviceRegistry = app.ApplicationServices.GetRequiredService<ServiceRegistry>();
            return serviceRegistry.DeregisterHealthCheckAsync(checkId)
                .Result;
        }
    }
}
