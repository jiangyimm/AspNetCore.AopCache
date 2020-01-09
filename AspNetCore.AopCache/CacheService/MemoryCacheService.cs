using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;

namespace AspNetCore.AopCache.CacheService
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public string GetCacheKey(MethodInfo method, ParameterInfo[] arguments, object[] values)
        {
            return new CacheKey(method, arguments, values).GetMemoryCacheKey();
        }

        public void SetValue(string key, object value, int expiration)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiration)
            });
        }

        public bool TryGetValue(string key, out object value, Type resultType)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public object GetValue(string key)
        {
            return _memoryCache.Get(key);
        }
    }
}
