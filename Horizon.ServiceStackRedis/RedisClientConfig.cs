using System;
using System.Collections.Generic;
using System.Text;

namespace Horizon.ServiceStackRedis
{
    public  class RedisClientConfig
    {

        #region Redis配置
        /// <summary>
        ///     读写的地址
        /// </summary>
        public  string[] ReadWriteServers = new string[] { "123ewq@localhost:6379" };//本地测试
        /// <summary>
        ///     只读地址
        /// </summary>
        public  string[] ReadOnlyServers = new string[] { "123ewq@localhost:6379" };//本地测试没做主从，所以读写同一个
        /// <summary>
        /// MaxWritePoolSize写的频率比读低
        /// </summary>
        public  int MaxWritePoolSize = 8;
        /// <summary>
        /// MaxReadPoolSize读的频繁比较多
        /// Redis最大连接数虽然官方是说1W，但是连接数要控制。
        /// </summary>
        public  int MaxReadPoolSize = 12;
        /// <summary>
        ///     连接最大的空闲时间 默认是240
        /// </summary>
        public  int IdleTimeOutSecs = 60;

        /// <summary>
        ///     连接超时时间，毫秒
        /// </summary>
        public  int ConnectTimeout = 6000;
        /// <summary>
        ///     数据发送超时时间，毫秒
        /// </summary>
        public  int SendTimeout = 6000;
        /// <summary>
        ///     数据接收超时时间，毫秒
        /// </summary>
        public  int ReceiveTimeout = 6000;
        /// <summary>
        ///     连接池取链接的超时时间，毫秒
        /// </summary>
        public  int PoolTimeout = 6000;
        /// <summary>
        ///     默认的数据库，暂无用。内部默认值也是0
        /// </summary>
        public  long DefaultDb = 0;
        #endregion


    }
}
