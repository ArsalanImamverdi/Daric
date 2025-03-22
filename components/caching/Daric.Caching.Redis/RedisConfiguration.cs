namespace Daric.Caching.Redis
{
    internal record RedisConfiguration()
    {
        public string ConnectionString { get; set; } = string.Empty;
    }
}
