using System.Collections.Generic;
using System.Linq;
using Horizon.Core.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Horizon.DataAccess.Repository
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>

        public static IServiceCollection AddHorizonORM(this IServiceCollection services, IConfiguration configuration, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, string section = "DbConfig")
        {

            MutiDbOperate connectOptions = new MutiDbOperate();

           configuration.GetSection(section).Bind(connectOptions);

            //var connectOptions = configuration.GetSection(section).Get<List<MutiDbOperate>>();
            if (connectOptions != null)
            {
                //new HorizonDataClient(connectOptions);
 
                    if (contextLifetime == ServiceLifetime.Scoped)
                        services.AddScoped(s => new HorizonDataClient(connectOptions));
                    if (contextLifetime == ServiceLifetime.Singleton)
                        services.AddSingleton(s => new HorizonDataClient(connectOptions));
                    if (contextLifetime == ServiceLifetime.Transient)
                        services.AddTransient(s => new HorizonDataClient(connectOptions));
                
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


        /// <summary>
        /// 添加数据库的仓库模式支持
        /// </summary>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public  static IServiceCollection AddHorizonDbRespository(this IServiceCollection services,IConfiguration config, ServiceLifetime contextLifetime=ServiceLifetime.Scoped,string section="DbConfig")
        {

            return  services;
        }
    }
}
