using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Grpc.Core;
using Horizon.Sample.Grpccontract;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using ProtoBuf.Grpc.Server;
using SkyApm.Utilities.DependencyInjection;

namespace Horizon.Sample.ConAPPService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("GRPC Server is running.......");

            CreateHostBuilder(args).Build().Run();

            ////非asp.net core的启动方式
            //const int Port = 50051;

            //Server server = new Server
            //{
            //    Ports = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
            //};
            //server.Services.AddCodeFirst(new MySevices());

            //server.Start();

            //Console.WriteLine("Greeter server listening on port " + Port);
            //Console.WriteLine("Press any key to stop the server...");
            //Console.ReadKey();

            //server.ShutdownAsync().Wait();


            //Console.WriteLine("Hello World!");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureServices(services =>
                        {
                            services.AddCodeFirstGrpc();
                            services.AddSkyApmExtensions();// add track
                            

                        })
                        .ConfigureKestrel(op =>
                        {
                            op.ListenLocalhost(10042, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                            //op.Listen(IPAddress.Any, ports.httpPort, listenOptions =>
                            //{
                            //    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                            //});
                            //op.Listen(IPAddress.Any, ports.grpcPort, listenOptions =>
                            //{
                            //    listenOptions.Protocols = HttpProtocols.Http2;
                            //});
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();

                            app.UseEndpoints(endpoints => { endpoints.MapGrpcService<MySevices>(); });
                        });
                });
    }

    public class MySevices:IStudentCollection
    {
        public ValueTask<Student> GetStudentAsync(ResponeConext conext)
        {
           return  new ValueTask<Student>(new Student
           {
               No = 1001,
               Age = 19,
               Name = conext.Name+"..come from..grpc services!"
           });
        }
    }
}
