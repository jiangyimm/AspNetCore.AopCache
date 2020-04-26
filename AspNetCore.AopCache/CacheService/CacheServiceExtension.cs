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
        public static void AddRedisCache(this IServiceCollection serviceCollection, Action<ICacheOption> option)
        {
            AddOption(serviceCollection, option);
            serviceCollection.AddSingleton<ICacheService, RedisCacheService>();
        }

        /// <summary>
        /// 使用Memory缓存
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="configure"></param>
        public static void AddMemoryCache(this IServiceCollection serviceCollection, Action<ICacheOption> option = null)
        {
            if (option != null)
            {
                AddOption(serviceCollection, option);
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
        public static void AddCustomCache<TCacheService>(this IServiceCollection serviceCollection, Action<ICacheOption> option = null)
         where TCacheService : class, ICacheService
        {
            if (option != null)
            {
                AddOption(serviceCollection, option);
            }
            serviceCollection.AddSingleton<ICacheService, TCacheService>();
        }

        private static void AddOption(this IServiceCollection serviceCollection, Action<ICacheOption> option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));
            var serviceDescriptor = serviceCollection.LastOrDefault(x =>
            {
                if (x.ServiceType == typeof(ICacheOption))
                    return x.ImplementationInstance != null;
                return false;
            });
            var implementationInstance = (ICacheOption)serviceDescriptor?.ImplementationInstance ?? new CacheOption();
            option(implementationInstance);
            if (serviceDescriptor == null)
                serviceCollection.AddSingleton(implementationInstance);
        }
    }
}
