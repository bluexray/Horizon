using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Horizon.Core.Data;
using Serilog;

namespace Horizon.DataAccess.Repository
{
    public  class HorizonDataClient
    {

        public HorizonDataClient(MutiDbOperate config)
        {
            Console.WriteLine("horizonDataclinet is start......");

            Func<List<SlaveConnectionConfig>>  action = () =>
            {
                var list = new List<SlaveConnectionConfig>();
                if (config.SlaveConnections.Count() < 1)
                {
                    throw new Exception("db config file is error!");
                }

                foreach (var item in config.SlaveConnections)
                {
                    list.Add(new SlaveConnectionConfig()
                    {
                        HitRate = item.HitRate,
                        ConnectionString = item.ConnectionString
                    });
                }

                return list;
            };


            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = config.ConnectionString, //Master Connection
                DbType = (DbType) config.DbType,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,//是否自动关闭连接
                IsShardSameThread = true,//共享线程
                SlaveConnectionConfigs = action()
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Log.Information(db.Ado.Connection.ConnectionString);
                Log.Information("sql: "+s);
                Log.Information(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
            };
        }

    }
}
