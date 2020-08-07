using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Serilog;
using Horizon.Consul;
using Horizon.Consul.Configurations;
using Horizon.Core.Router;
using Horizon.Core.Services;
using Microsoft.Extensions.Configuration;

namespace Horizon.GRPC
{
    public static class GrpcCallerService
    {

        

        public static async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<GrpcChannel, Task<TResponse>> func)
        {

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            //GrpcClientFactory.AllowUnencryptedHttp2 = true;

            var channel = GrpcChannel.ForAddress(urlGrpc);

            /*
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };

            */
            

            Log.Information("Creating grpc client base address urlGrpc ={@urlGrpc}, BaseAddress={@BaseAddress}  ", urlGrpc, channel.Target);


            try
            {
                return await func(channel);
            }
            catch (RpcException e)
            {
                //Log.Error("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
                return default;
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }


        }

        public static async Task CallService(string urlGrpc, Func<GrpcChannel, Task> func)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            /*
            using var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };
            */

            var channel = GrpcChannel.ForAddress(urlGrpc); 
            
            Log.Debug("Creating grpc client base address {@httpClient.BaseAddress} ", channel.Target);

            try
            {
                await func(channel);
            }
            catch (RpcException e)
            {
                Log.Error("Error calling via grpc: {Status} - {Message} ", e.Status, e.Message);
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", false);
            }
        }

        public static dynamic GetGrpcServicesHosts(string serviceName,string serviceTags,IConfiguration configuration)
        {

            ConsulServiceDiscoveryOption serviceDiscoveryOption = new ConsulServiceDiscoveryOption();
            configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);

            var proxy = new ConsulProxy(serviceDiscoveryOption.Consul);

            var rs = proxy.FindServiceInstancesAsync(serviceName,serviceTags);


            LoadBalancerFactory loader = new LoadBalancerFactory(rs.Result);
           var balancer= loader.Get(serviceName, LoadBalancerMode.Random).Result;

          var reslut= balancer.SelectManyAsync();

            var  response =new 
            {
                host = reslut.Result
            };

            return response;
        }
        
    }
}
