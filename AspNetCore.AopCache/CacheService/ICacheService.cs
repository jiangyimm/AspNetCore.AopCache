using System;
using System.Reflection;

namespace AspNetCore.AopCache.CacheService
{
    /// <summary>
    /// 缓存抽象接口
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 获取缓存key
        /// </summary>
        /// <param name="method"></param>
        /// <param name="arguments"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        string GetCacheKey(MethodInfo method, ParameterInfo[] arguments, object[] values);

        /// <summary>
        /// 把对象加入缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiration">有效期 分钟</param>
        void SetValue(string key, object value, int? expiration);

        /// <summary>
        /// 尝试获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="resultType"></param>
        /// <returns></returns>
        bool TryGetValue(string key, out object value, Type resultType);

        /// <summary>
        /// 从缓存取值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetValue(string key);
    }
}
