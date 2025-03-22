using StackExchange.Redis;

namespace Daric.Locking.MedallionRedis
{
    /// <summary>
    /// Options for the implementation of the DistributedLock using Redis (RedLock algorithm) by this library <see href="https://github.com/madelson/DistributedLock"/>
    /// </summary>
    public class MedallionRedisLockOptions(IDatabase database)
    {
        /// <summary>
        /// Default timeout when the Acquire methods try to acquire a lock
        /// </summary>
        public static readonly TimeSpan DefaultWaitTimeToAcquireLock = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Selected database of the Redis instance[s]
        /// </summary>
        public IDatabase Database { get; } = database;


        /// <summary>
        /// Default timeout for connecting to Redis instance[s]
        /// </summary>
        public const int DefaultConnectionTimeoutInMilliseconds = 5000;


    }
}
