using System;
using System.Collections.Generic;
using System.Text;
using Horizon.GRPC.Interceptors;
using Horizon.GRPC.Track;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTracing;
using Jaeger.Senders.Thrift;

namespace Horizon.GRPC
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHorizonGrpc<TStartup>(this IServiceCollection services,
            IConfiguration conf)
        {


            //添加服务端中间件
            services.AddServerInterceptor<MonitorInterceptor>();
         
            
            //Jaeger
            services.AddJaeger(conf);
         


            //添加GrpcClient扩展
            //services.AddGrpcClientExtensions(conf);

            return services;
        }




        /// <summary>
        /// 添加Jaeger和Interceptor
        /// </summary>
        /// <param name="services"></param>
        /// <param name="conf">JaegerOptions</param>
        /// <returns></returns>
        private static IServiceCollection AddJaeger(this IServiceCollection services, IConfiguration conf)
        {
            var key = "Jaeger";
            var jaegerOptions = conf.GetSection(key).Get<JaegerOptions>();
            if (jaegerOptions == null || jaegerOptions.Enable == false)
                return services;

            //jaeger
            services.AddSingleton<ITracer>(sp => {
                var options = sp.GetService<IOptions<JaegerOptions>>().Value;
                var serviceName = options.ServiceName;
                var tracer = new Jaeger.Tracer.Builder(serviceName)
                    .WithLoggerFactory(sp.GetService<ILoggerFactory>())
                    .WithSampler(new Jaeger.Samplers.ConstSampler(true))
                    .WithReporter(new Jaeger.Reporters.RemoteReporter.Builder()
                        .WithFlushInterval(TimeSpan.FromSeconds(5))
                        .WithMaxQueueSize(5)
                        .WithSender(new UdpSender(jaegerOptions.AgentIp, jaegerOptions.AgentPort, 1024 * 5)).Build())
                    .Build();
                return tracer;
            });
            //添加jaeger中间件
            services.AddServerInterceptor<JaegerTracingInterceptor>();

            return services;
        }

        /// <summary>
        /// 添加服务端Interceptor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServerInterceptor<T>(this IServiceCollection services) where T : ServerInterceptor
        {
            services.AddSingleton<ServerInterceptor, T>();

            return services;
        }
    }

}
