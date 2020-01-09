using Microsoft.Extensions.DependencyInjection;
using System;

namespace AspNetCore.AopCache.FrameworkBase
{
    /// <summary>
    /// DI 辅助类，全局获取
    /// </summary>
    public class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static IServiceScope CreateServiceScope()
        {
            return _serviceProvider.CreateScope();
        }

        public static IServiceProvider CreateServiceProvider()
        {
            return CreateServiceScope().ServiceProvider;
        }

        /// <summary>
        /// 获取服务，未找到抛异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetRequiredService<T>()
            where T : class
        {
            return CreateServiceProvider().GetRequiredService<T>();
        }

        /// <summary>
        /// 获取服务，未找到返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetService<T>()
            where T : class
        {
            return CreateServiceProvider().GetService<T>();
        }
    }
}
