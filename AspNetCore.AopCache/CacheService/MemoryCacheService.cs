using AspNetCore.AopCache.Configuration;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;

namespace AspNetCore.AopCache.CacheService
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICacheOption _cacheConfiguration;
        public MemoryCacheService(IMemoryCache memoryCache,
            ICacheOption cacheConfiguration)
        {
            _memoryCache = memoryCache;
            _cacheConfiguration = cacheConfiguration;
        }

        public virtual string GetCacheKey(MethodInfo method, ParameterInfo[] arguments, object[] values)
        {
            return new CacheKey(method, arguments, values).GetMemoryCacheKey();
        }

        public virtual void SetValue(string key, object value, int? expiration)
        {
            _memoryCache.Set(key, value, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiration ?? _cacheConfiguration.Expiration)
            });
        }

        public virtual bool TryGetValue(string key, out object value, Type resultType)
        {
            return _memoryCache.TryGetValue(key, out value);
        }

        public virtual object GetValue(string key)
        {
            return _memoryCache.Get(key);
        }
    }
}
