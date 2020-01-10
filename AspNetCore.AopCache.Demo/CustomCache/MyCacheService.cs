using AspNetCore.AopCache.CacheService;
using System;
using System.Reflection;

namespace AspNetCore.AopCache.Demo.CustomCache
{
    public class MyCacheService : ICacheService
    {

        public string GetCacheKey(MethodInfo method, ParameterInfo[] arguments, object[] values)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string key, object value, int? expiration)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value, Type resultType)
        {
            throw new NotImplementedException();
        }

        public object GetValue(string key)
        {
            throw new NotImplementedException();
        }
    }
}
