using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Horizon.GRPC;
using Horizon.Sample.Grpccontract;
using ProtoBuf.Grpc;
using ProtoBuf.Grpc.Client;
using Serilog;

namespace Horizon.Sample.ConAppClinet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                //.WriteTo.Console()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();


            await GrpcCallerService.CallService("http://localhost:10042", async channel =>
             {
                 var clinet = channel.CreateGrpcService<IStudentCollection>();
                 var rs = await clinet.GetStudentAsync(new ResponeConext
                 {
                     Name = "Peter"
                 });
                
                 Console.WriteLine($"学生学号: {rs.No}" + $"学生姓名: {rs.Name} " + $"学生性别: {rs.Age}");
                 Console.ReadKey();
                 return rs;
             });

            #region 常规写法

            

           
            //GrpcClientFactory.AllowUnencryptedHttp2 = true;

            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            

            //using var http = GrpcChannel.ForAddress("http://localhost:10042");
            //var token = "";
            //var headers = new Metadata { { "Authorization", $"Bearer {token}" } };

            //var  service = http.CreateGrpcService<IStudentCollection>();


            ////using var cancel = new CancellationTokenSource(TimeSpan.FromMinutes(1));
            ////var options = new CallOptions(cancellationToken: cancel.Token);



            //var  result= await service.GetStudentAsync(new ResponeConext
            //{
            //    Name = "Tommy"
            //});

            //Console.WriteLine($"学生学号: {result.No}"+$"学生姓名: {result.Name} "+$"学生性别: {result.Age}");
            //Console.ReadKey();
            #endregion
        }
    }
}
