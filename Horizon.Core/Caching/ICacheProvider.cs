using System;
using System.Collections.Generic;
using System.Text;

namespace Horizon.Core.Caching
{
    public interface ICacheProvider
    {
        public  void SetValue(string key,string value,int expirySeconds);
        public  string GetValue(string key);
        public  bool Remove(string key);
        public  void RemoveAll(string[] keys);


        public T Store<T>(T entity);
        public void StoreAll<T>(IEnumerable<T> entities);
        public object StoreObject(object entity);
        public T GetById<T>(object id);
        public List<T> GetValues<T>(List<string> keys);
        public IList<T> GetAll<T>();
        public void DeleteAll<T>();
        public void DeleteById<T>(object id);
        
    }
}
