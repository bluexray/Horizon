using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Horizon.Sample.Grpccontract;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;

namespace Horizon.Sample.ConAppClinet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            

            using var http = GrpcChannel.ForAddress("http://localhost:10042");
            var token = "";
            var headers = new Metadata { { "Authorization", $"Bearer {token}" } };

            var  service = http.CreateGrpcService<IStudentCollection>();


            //using var cancel = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            //var options = new CallOptions(cancellationToken: cancel.Token);



            var  result= await service.GetStudentAsync(new ResponeConext
            {
                Name = "Tommy"
            });

            Console.WriteLine($"学生学号: {result.No}"+$"学生姓名: {result.Name} "+$"学生性别: {result.Age}");
            Console.ReadKey();

        }
    }
}
