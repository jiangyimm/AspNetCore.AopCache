namespace AspNetCore.AopCache.Configuration
{
    public interface ICacheConfiguration
    {
        /// <summary>
        /// 缓存有限期，单位：分钟，默认值：10
        /// </summary>
        int Expiration { get; set; }

        /// <summary>
        /// redis服务地址
        /// </summary>
        string RedisHost { get; set; }
    }
}
