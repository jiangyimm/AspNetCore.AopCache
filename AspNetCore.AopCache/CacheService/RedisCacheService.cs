using AspNetCore.AopCache.Configuration;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Reflection;

namespace AspNetCore.AopCache.CacheService
{
    public class RedisCacheService : ICacheService
    {
        private readonly ICacheConfiguration _cacheConfiguration;
        public RedisCacheService(ICacheConfiguration cacheConfiguration)
        {
            _cacheConfiguration = cacheConfiguration;
        }

        private RedisManagerPool _redisManagerPool;
        public RedisManagerPool RedisManagerPool => _redisManagerPool ?? (_redisManagerPool = new RedisManagerPool(_cacheConfiguration.RedisHost));
        public IRedisClient RedisClient => RedisManagerPool.GetClient();
        public virtual string GetCacheKey(MethodInfo method, ParameterInfo[] arguments, object[] values)
        {
            return new CacheKey(method, arguments, values).GetRedisCacheKey();
        }

        public virtual void SetValue(string key, object value, int? expiration)
        {
            try
            {
                using (var redisClient = RedisManagerPool.GetClient())
                {
                    redisClient.Set(key, value, new TimeSpan(0, expiration ?? _cacheConfiguration.Expiration, 0));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }

        public virtual bool TryGetValue(string key, out object value, Type resultType)
        {
            try
            {
                using (var redisClient = RedisManagerPool.GetClient())
                {
                    value = null;
                    var json = redisClient.GetValue(key);
                    if (json != null)
                    {
                        value = JsonConvert.DeserializeObject(json, resultType);
                    }
                    return value != null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                value = null;
                return false;
            }
        }

        public virtual object GetValue(string key)
        {
            try
            {
                using (var redisClient = RedisManagerPool.GetClient())
                {
                    var value = redisClient.Get<object>(key);
                    return value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
                return null;
            }
        }
    }
}
