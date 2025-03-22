using Daric.Caching.Abstractions;
using Daric.Caching.Redis.Internals;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using StackExchange.Redis;

namespace Daric.Caching.Redis
{
    public static class RedisExtensions
    {
        public static IHostApplicationBuilder AddRedis(this IHostApplicationBuilder host)
        {
            host.Services.AddOptions<RedisConfiguration>().Configure<IConfiguration>((redisConfiguration, config) =>
            {
                redisConfiguration.ConnectionString = config.GetSection("Config:Redis:ConnectionStrings").Value ?? "localhost:6379";
            });
            host.Services.AddOptions<ConnectionMultiplexerConnect>().Configure<IOptionsMonitor<RedisConfiguration>>((connectionMultiplexer, redisConfiguration) =>
            {
                connectionMultiplexer.ConnectTask = ConnectionMultiplexer.ConnectAsync(redisConfiguration.CurrentValue.ConnectionString);
            });
            host.Services.AddScoped<IRedisDatabaseProvider, RedisDatabaseProvider>();
            host.Services.AddScoped<IDistributedCacheDatabase, RedisDatabase>();
           
            return host;
        }

    }
}
