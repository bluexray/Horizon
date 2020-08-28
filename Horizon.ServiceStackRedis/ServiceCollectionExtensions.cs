using Horizon.Core.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;

namespace Horizon.ServiceStackRedis
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddHorizonRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisClientConfig>(configuration.GetSection("redis"));
            services.AddSingleton<ICacheProvider, RedisCacheProvider>();
            return services;
        }
    }
}
