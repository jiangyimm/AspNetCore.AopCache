using AspNetCore.AopCache.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AspNetCore.AopCache.CacheService
{
    public static class CacheServiceExtension
    {
        /// <summary>
        /// 使用Redis缓存
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configure"></param>
        public static void UseRedisCache(this IServiceCollection serviceCollection, Action<ICacheConfiguration> configure)
        {
            AddConfiguration(serviceCollection, configure);
            serviceCollection.AddSingleton<ICacheService, RedisCacheService>();
        }

        /// <summary>
        /// 使用Memory缓存
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configure"></param>
        public static void UseMemoryCache(this IServiceCollection serviceCollection, Action<ICacheConfiguration> configure = null)
        {
            if (configure != null)
            {
                AddConfiguration(serviceCollection, configure);
            }
            serviceCollection.AddMemoryCache();
            serviceCollection.AddSingleton<ICacheService, MemoryCacheService>();
        }

        /// <summary>
        /// 使用自定义缓存
        /// </summary>
        /// <typeparam name="TCacheService">自定义的缓存实现</typeparam>
        /// <param name="serviceCollection"></param>
        /// <param name="configure"></param>
        public static void UseCustomCache<TCacheService>(this IServiceCollection serviceCollection, Action<ICacheConfiguration> configure = null)
         where TCacheService : class, ICacheService
        {
            if (configure != null)
            {
                AddConfiguration(serviceCollection, configure);
            }
            serviceCollection.AddSingleton<ICacheService, TCacheService>();
        }

        private static void AddConfiguration(this IServiceCollection serviceCollection, Action<ICacheConfiguration> configure)
        {
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            var serviceDescriptor = serviceCollection.LastOrDefault(x =>
            {
                if (x.ServiceType == typeof(ICacheConfiguration))
                    return x.ImplementationInstance != null;
                return false;
            });
            var implementationInstance = (ICacheConfiguration)serviceDescriptor?.ImplementationInstance ?? new CacheConfiguration();
            configure(implementationInstance);
            if (serviceDescriptor == null)
                serviceCollection.AddSingleton(implementationInstance);
        }
    }
}
