using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Hosting;

namespace Horizon.Sample.GrpcServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://localhost:5021");
            webBuilder.ConfigureKestrel((context, options) =>
            {
                options.Limits.MinRequestBodyDataRate = null;
            });
        });
        //.ConfigureAppConfiguration((hostingContext, _config) =>
        //{
        //    _config.Add(new JsonConfigurationSource { Path = "config/servers.json", ReloadOnChange = true })
        //                    .Build();
        //})

    }
}
