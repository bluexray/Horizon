using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Horizon.Consul.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Horizon.Sample.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    

                    webBuilder.ConfigureAppConfiguration((host, config) =>
                               {
                                   var  env =host.HostingEnvironment;
                                   //����consul��Ϊע������
                                   config.AddHorizonConsulCenter("Horizon/Horizon.MySql", ops =>
                                   {
                                       ops.ConsulConfigurationOptions = co => { co.Address = new Uri("http://172.16.100.198:8500/"); };
                                       ops.Optional = true;
                                       ops.ReloadOnChange = true;//���º����¼���
                                       ops.OnLoadException = exp => { exp.Ignore = true; };
                                   });
                                   host.Configuration = config.Build();
                               });
                    webBuilder.UseStartup<Startup>();

                }
                );
    }
}
