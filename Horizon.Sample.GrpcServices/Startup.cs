using Horizon.Consul;
using Horizon.Consul.Configurations;
using Horizon.GRPC;
using Horizon.GRPC.Interceptors;
using Horizon.Sample.GrpcServices.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkyApm.Utilities.DependencyInjection;
using System;

namespace Horizon.Sample.GrpcServices
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc(optons =>
            {
                optons.Interceptors.Add<JaegerTracingInterceptor>();//注册Grpc全局拦截器
            });

            //注册gRPC服务 + 单个服务拦截
            //services.AddGrpc().AddServiceOptions<GreeterService>(options => {
            //    options.Interceptors.Add<MyServerInterceptor1>();
            //});

            



            services.AddSkyApmExtensions();


            IConfiguration config = new ConfigurationBuilder()
                                    .Add(new JsonConfigurationSource { Path = "config/servers.json", ReloadOnChange = true })
                                    .Build();

            var result = config.GetSection("ServiceDiscovery")["ServiceName"];

            //services.Configure<ConsulHostConfiguration>()

            //configuration.GetSection("ServiceDiscovery: Consul")
            //services.Configure<ConsulHostConfiguration>(.GetSection("ServiceDiscovery:Consul"));

            //services.AddHorizonConsul(new ConfigurationBuilder().AddJsonFile("config\\servers.json").Build());


            services.AddHorizonConsul(Configuration);//增加consul的配置

            services.AddHorizonGrpc<Startup>(config);


            ConsulServiceDiscoveryOption host = new ConsulServiceDiscoveryOption();
            config.GetSection("ServiceDiscovery").Bind(host);

            var serviecname = host.ServiceName;
            var s = host.Consul.HttpEndpoint;

            Console.WriteLine($"servicename:{serviecname}");
            Console.WriteLine($"url:{s}");

            //services.AddCors(options => options.AddPolicy
            //                    (
            //                    "HorizonCors",
            //                    p => p.SetIsOriginAllowedToAllowWildcardSubdomains()
            //                        .WithOrigins("https://*.cnblogs.com", "http://*.cnblogs.com")
            //                        .AllowAnyMethod().AllowAnyHeader()
            //                        )
            //                    );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHorizonConsul(Configuration);//使用consul

           

            app.UseRouting();

            // 添加健康检查路由地址
            app.Map("/health", HealthMap);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<GreeterService>();
                endpoints.MapGrpcService<HealthCheckService>();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }

        private static void HealthMap(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("OK");
            });
        }
    }
}
