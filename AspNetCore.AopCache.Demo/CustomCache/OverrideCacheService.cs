using AspNetCore.AopCache.CacheService;
using AspNetCore.AopCache.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetCore.AopCache.Demo.CustomCache
{
    public class OverrideCacheService : MemoryCacheService
    {
        public OverrideCacheService(IMemoryCache memoryCache, ICacheOption cacheConfiguration) : base(memoryCache, cacheConfiguration)
        {
        }

        public override void SetValue(string key, object value, int? expiration)
        {
            base.SetValue(key, value, expiration);
        }
    }
}
