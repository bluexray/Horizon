using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

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
                 outputTemplate: "发生时间:{Timestamp: HH:mm:ss.fff} 事件级别:{Level} 详细信息:{Message}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine("Logs", "log.log"), rollingInterval: RollingInterval.Day)//文件生成到当前路径 rollingInterval:RollingInterval.Day:按天生成文件
                .CreateLogger();
            
            Log.Information("serilog is config.......");

            CreateHostBuilder(args)
                .UseConsoleLifetime()
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

                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("config\\servers.json", false, true);
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
            webBuilder.UseUrls("http://172.16.50.72:5021");
            webBuilder.ConfigureKestrel((context, options) =>
            {
                options.Limits.MinRequestBodyDataRate = null;
            });

            //配置grpc的协议端口
            webBuilder.UseKestrel(ops =>
            {
                ops.ListenAnyIP(5021, p => p.Protocols =
                    Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2);
            });
            //配置http健康检查协议端口
            webBuilder.UseKestrel(o=>
            {
                o.ListenAnyIP(5001);
            });
        });
        //.ConfigureAppConfiguration((hostingContext, _config) =>
        //{
        //    _config.Add(new JsonConfigurationSource { Path = "config/servers.json", ReloadOnChange = true })
        //                    .Build();
        //})

    }
}
