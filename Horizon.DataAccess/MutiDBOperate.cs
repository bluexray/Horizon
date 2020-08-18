using System;
using System.Collections.Generic;
using System.Text;
using Horizon.DataAccess;

namespace Horizon.Core.Data
{
    [Serializable]
    public class MutiDbOperate
    {
        /// <summary>
        /// 数据库连接名称
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 从库执行级别，越大越先执行
        /// </summary>
        public int HitRate { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public DataBaseType DbType { get; set; }

        public List<SlaveConnectionConfig> SlaveConnections { get; set; }
    }

    public enum DataBaseType
    {
        MySql = 0,
        SqlServer = 1,
        Sqlite = 2,
        Oracle = 3,
        PostgreSQL = 4
    }
}
