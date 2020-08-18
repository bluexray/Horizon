using System;
using System.Net.Http;
using System.Threading.Channels;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using Horizon.GRPC;
using Horizon.Sample.GrpcServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Horizon.Sample.GrpcClient
{
    class  Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);

            var httpClientHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);



            IConfiguration config = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "config/servers.json", ReloadOnChange = true })
                .Build();

           var  url=GrpcServiceManager.GetGrpcServicesHosts("Horizon.Sample.GrpcServices", "", config);

            url = "http://" + url;

            var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions { HttpClient = httpClient });

            //var channel = new Channel("127.0.0.1:50051", ChannelCredentials.Insecure);

            var clinet = new Greeter.GreeterClient(channel);


         var  reply = await clinet.SayHelloAsync(new HelloRequest
            {
                Name = "Toomy"
            });



        Console.WriteLine("Hello World!"+reply.Message);

        await channel.ShutdownAsync();
         Console.ReadKey();
        }
    }
}
