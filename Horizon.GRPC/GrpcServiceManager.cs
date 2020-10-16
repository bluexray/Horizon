using Grpc.Core;
using Grpc.Net.Client;
using Horizon.Consul;
using Horizon.Consul.Configurations;
using Horizon.Core.Router;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Horizon.GRPC
{
    public static class GrpcServiceManager
    {

        

        public static async Task<TResponse> CallService<TResponse>(string urlGrpc, Func<GrpcChannel, Task<TResponse>> func)
        {

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);



            //设置发送和接送数据大小
            var  options = new GrpcChannelOptions
            { 
                 MaxReceiveMessageSize = int.MaxValue,
                 MaxSendMessageSize = int.MaxValue
            };

            //GrpcClientFactory.AllowUnencryptedHttp2 = true;

            var channel = GrpcChannel.ForAddress(urlGrpc,options);
            
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
                Log.Error("Error calling via grpc: {Status} - {Message}", e.Status, e.Message);
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

        public static string
            GetGrpcServicesHosts(string serviceName,string serviceTags,IConfiguration configuration)
        {
            
            ConsulServiceDiscoveryOption serviceDiscoveryOption = new ConsulServiceDiscoveryOption();
            configuration.GetSection("ServiceDiscovery").Bind(serviceDiscoveryOption);

            var proxy = new ConsulProxy(serviceDiscoveryOption.Consul);


            var r = proxy.FindAllServicesAsync().Result;

            var q=  r.FirstOrDefault(x => x.Name == serviceName);
            if (q==null)
            {
                return "127.0.0.1:8600";
            }

            return q.HostAndPort.ToString();


            var rs = proxy.FindServiceInstancesAsync(serviceName,serviceTags);


            LoadBalancerFactory loader = new LoadBalancerFactory(rs.Result);
            var balancer= loader.Get(serviceName, LoadBalancerMode.Random).Result;

            var reslut= balancer.SelectManyAsync();



            return reslut.Result.ToString();
        }
        
    }
}
