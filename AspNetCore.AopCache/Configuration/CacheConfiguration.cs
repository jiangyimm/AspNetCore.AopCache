namespace AspNetCore.AopCache.Configuration
{
    public class CacheConfiguration : ICacheConfiguration
    {
        public int Expiration { get; set; } = 10;
        public string RedisHost { get; set; }
    }
}
