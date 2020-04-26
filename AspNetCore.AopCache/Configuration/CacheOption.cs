namespace AspNetCore.AopCache.Configuration
{
    public class CacheOption : ICacheOption
    {
        public int Expiration { get; set; } = 10;
        public string RedisHost { get; set; }
    }
}
