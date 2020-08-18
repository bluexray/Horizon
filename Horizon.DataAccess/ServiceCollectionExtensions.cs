using System.Collections.Generic;
using System.Linq;
using Horizon.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Horizon.DataAccess
{
    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddHorizonORM(this IServiceCollection services, IConfiguration configuration, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, string section = "DbConfig")
        {

            MutiDbOperate dboption = new MutiDbOperate();

           var result =configuration.GetSection("DbConfig");//.Bind(dboption);


           var jwt = configuration.GetSection("JwtAuth")["Audience"].ToString();

           var h = result["HitRate"].ToString();
           var s = result["ConnectionString"].ToString();

           result.Bind(dboption);


            _ = dboption.DbName;
           //services.Configure<MutiDbOperate>(configuration.GetSection(section));
            var connectOptions = configuration.GetSection(section).Get<List<MutiDbOperate>>();
            if (connectOptions != null)
            {

                foreach (var option in connectOptions)
                {
                    if (contextLifetime == ServiceLifetime.Scoped)
                        services.AddScoped(s => new HorizonDataClient(option));
                    if (contextLifetime == ServiceLifetime.Singleton)
                        services.AddSingleton(s => new HorizonDataClient(option));
                    if (contextLifetime == ServiceLifetime.Transient)
                        services.AddTransient(s => new HorizonDataClient(option));
                }
            }

            return services;
        }


        /// <summary>
        /// 添加sqlSugar多数据库支持
        /// </summary>
        /// <param name="services"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static IServiceCollection AddHorizonSqlContext(this IServiceCollection services, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, string section = "DbConfig")
        {

            
            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration)service.ImplementationInstance;
            return AddHorizonORM(services, configuration, contextLifetime, section);
        }


    }
}
