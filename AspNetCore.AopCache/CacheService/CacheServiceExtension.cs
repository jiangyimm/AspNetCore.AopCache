using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.AopCache.CacheService
{
    public static class CacheServiceExtension
    {
        /// <summary>
        /// 使用Redis缓存
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void UseRedisCache(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ICacheService, RedisCacheService>();
        }

        /// <summary>
        /// 使用Memory缓存
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void UseMemoryCache(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton<ICacheService, MemoryCacheService>();
        }
    }
}
