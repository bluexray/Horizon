using Horizon.Core.Caching;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Horizon.StackExchangeRedis
{
    public class RedisHelper:ICacheProvider
    {
        private int DbNum { get; }
        private readonly ConnectionMultiplexer _conn;
        public string CustomKey;

        #region 构造函数

        public RedisHelper(int dbNum = 0)
                : this(dbNum, null)
        {
        }

        public RedisHelper(int dbNum, string readWriteHosts)
        {
            DbNum = dbNum;
            _conn =
                string.IsNullOrWhiteSpace(readWriteHosts) ?
                RedisConnectionHelp.Instance :
                RedisConnectionHelp.GetConnectionMultiplexer(readWriteHosts);
        }

        #region 辅助方法

        private string AddSysCustomKey(string oldKey)
        {
            var prefixKey = CustomKey ?? RedisConnectionHelp.SysCustomKey;
            return prefixKey + oldKey;
        }

        private T Do<T>(Func<IDatabase, T> func)
        {
            var database = _conn.GetDatabase(DbNum);
            return func(database);
        }

        private string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        private T ConvertObj<T>(RedisValue value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        private List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }

        private RedisKey[] ConvertRedisKeys(List<string> redisKeys)
        {
            return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
        }


        #endregion 辅助方法

        #endregion 构造函数


        public void SetValue(string key, string value, int expirySeconds)
        {
            throw new NotImplementedException();
        }

        public string GetValue(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll(string[] keys)
        {
            throw new NotImplementedException();
        }

        public T Store<T>(T entity)
        {
            throw new NotImplementedException();
        }

        public void StoreAll<T>(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public object StoreObject(object entity)
        {
            throw new NotImplementedException();
        }

        public T GetById<T>(object id)
        {
            throw new NotImplementedException();
        }

        public List<T> GetValues<T>(List<string> keys)
        {
            throw new NotImplementedException();
        }

        public IList<T> GetAll<T>()
        {
            throw new NotImplementedException();
        }

        public void DeleteAll<T>()
        {
            throw new NotImplementedException();
        }

        public void DeleteById<T>(object id)
        {
            throw new NotImplementedException();
        }


    }
}