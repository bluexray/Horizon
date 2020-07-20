using System;
using System.Net.Http;
using System.Threading.Channels;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using Horizon.Sample.GrpcServices;

namespace Horizon.Sample.GrpcClient
{
    class  Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            var httpClientHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpClientHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);


            //AppContext.SetSwitch(
            //    "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var channel = GrpcChannel.ForAddress("https://127.0.0.1:5001",new GrpcChannelOptions { HttpClient = httpClient });

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
