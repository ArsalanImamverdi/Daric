using Daric.Caching.Redis;
using Daric.Locking.Abstraction;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Daric.Locking.MedallionRedis
{
    public static class DistributedLockingOptionsExtensions
    {
        public static DistributedLockingOptions AddMedallionRedisLockMechanism(this DistributedLockingOptions lockingOptions, string providerName = "")
        {
            var serviceCollection = lockingOptions.ServiceCollection;
            serviceCollection.Configure<DefaultMedallionRedisLockConfig>(serviceCollection.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("Config:LockingConfig"));
            return lockingOptions.AddMedallionRedisLockMechanism(providerName, null);
        }
        public static DistributedLockingOptions AddMedallionRedisLockMechanism(this DistributedLockingOptions lockingOptions, string providerName, MedallionRedisLockOptions? options)
        {
            var serviceCollection = lockingOptions.ServiceCollection;
            serviceCollection.AddScoped<IDistributedLockMechanism>(serviceProvider =>
            {
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(500);
                try
                {
                    options ??= new MedallionRedisLockOptions(serviceProvider.GetRequiredService<IRedisDatabaseProvider>().GetDatabaseAsync(cancellationTokenSource.Token).GetAwaiter().GetResult());
                }
                catch (TaskCanceledException ex)
                {
                    var logger = serviceProvider.GetService<ILogger<DistributedLockingOptions>>();
                    logger?.LogWarning("Can not connect to Redis in Daric.Locking");
                }
                if (options is null)
                    return default!;

                return new MedallionRedisLockMechanism(providerName, options);
            });
            return lockingOptions;
        }
    }
}
