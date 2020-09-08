using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Horizon.Consul.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddHorizonConsulCenter(
            this IConfigurationBuilder builder,
            string key)
        {
            return builder.AddHorizonConsulCenter(key, options => { });
        }

        /// <summary>
        /// 读取Consul中的配置
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="key"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IConfigurationBuilder AddHorizonConsulCenter(
            this IConfigurationBuilder builder,
            string key,
            Action<IConsulConfigurationSource> options)
        {
            
            var consulConfigSource = new ConsulConfigurationSource(key);
            options(consulConfigSource);
            return builder.Add(consulConfigSource);
        }
    }
}
