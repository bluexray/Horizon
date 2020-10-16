using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Runtime.InteropServices;

namespace Horizon.Sample.GrpcServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console(theme: AnsiConsoleTheme.Code,
                 outputTemplate: "����ʱ��:{Timestamp: HH:mm:ss.fff} �¼�����:{Level} ��ϸ��Ϣ:{Message}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine("Logs", "log.log"), rollingInterval: RollingInterval.Day)//�ļ����ɵ���ǰ·�� rollingInterval:RollingInterval.Day:���������ļ�
                .CreateLogger();

            Log.Information("serilog is config.......");

            CreateHostBuilder(args)
                .UseConsoleLifetime()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(logger =>
                {

                    //logger.AddFilter("Microsoft", LogLevel.Critical)
                    //    .AddFilter("System", LogLevel.Critical);
                })
                .Build().Run();

        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .ConfigureAppConfiguration((context,config) =>
                {
                    string url = context.HostingEnvironment.ContentRootPath;
                    var  rs= Directory.GetCurrentDirectory();
                    Log.Information($@"rootpath :{url}" );
                    //var  rs =RuntimeEnvironment.GetRuntimeDirectory();
                    
                    config.AddJsonFile($"{rs}/Config/servers.json", false, true);
                })

        //.ConfigureWebHostDefaults(options =>
        //{
        //    options.ConfigureKestrel(o =>
        //    {
        //        o.ListenLocalhost(5021, p => p.Protocols =
        //            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);

        //    });
        //})
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseSerilog();
            webBuilder.UseStartup<Startup>();

        
            webBuilder.ConfigureKestrel((context, options) =>
            {
                options.Limits.MinRequestBodyDataRate = null;
            });

            //����grpc��Э��˿�
            webBuilder.UseKestrel(ops =>
            {
                ops.Listen(IPAddress.Any, 5021, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                            //listenOptions.UseHttps("<path to .pfx file>", 
                            //    "<certificate password>");
                        });
            });

            ////����http�������Э���mvc�˿�
            webBuilder.UseKestrel(o =>
            {
                o.ListenAnyIP(5025);
            });
        });
        //.ConfigureAppConfiguration((hostingContext, _config) =>
        //{
        //    _config.Add(new JsonConfigurationSource { Path = "config/servers.json", ReloadOnChange = true })
        //                    .Build();
        //})

    }
}
