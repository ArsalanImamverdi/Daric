
using Daric.Locking.Abstraction;

using Medallion.Threading.Redis;

namespace Daric.Locking.MedallionRedis
{
    /// <summary>
    /// An implementation/wrapper of the DistributedLock using Redis (RedLock algorithm) by this library : <see href="https://github.com/madelson/DistributedLock"/>
    /// </summary>
    public class MedallionRedisLockMechanism : BaseDistributedLockMechanism<MedallionRedisLockOptions>
    {
        public MedallionRedisLockMechanism(string providerName, MedallionRedisLockOptions options)
            : base(providerName, options)
        {
        }

        public override bool IsLockExpirySupport => true;

        public override IDistributedSynchronizationHandle Acquire(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default)
        {
            var @lock = ValidateResourceNameThenPrepareWaitTimeThenGenerateLock(resourceName, lockExpiry, ref waitTimeToAcquireLock);
            var handle = @lock.Acquire(timeout: waitTimeToAcquireLock, cancellationToken: cancellationToken);

            return new DistributedSynchronizationHandle(handle);
        }

        public override async ValueTask<IDistributedSynchronizationHandle> AcquireAsync(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default)
        {
            var @lock = ValidateResourceNameThenPrepareWaitTimeThenGenerateLock(resourceName, lockExpiry, ref waitTimeToAcquireLock);
            var handle = await @lock.AcquireAsync(timeout: waitTimeToAcquireLock, cancellationToken: cancellationToken);

            return new DistributedSynchronizationHandle(handle);
        }

        public override IDistributedSynchronizationHandle? TryAcquire(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default)
        {
            var @lock = ValidateResourceNameThenPrepareWaitTimeThenGenerateLock(resourceName, lockExpiry, ref waitTimeToAcquireLock);
#pragma warning disable CS8629 // Nullable value type may be null.
            var handle = @lock.TryAcquire(timeout: waitTimeToAcquireLock.Value, cancellationToken: cancellationToken);
#pragma warning restore CS8629 // Nullable value type may be null.

            return handle is not null ? new DistributedSynchronizationHandle(handle) : null;
        }

        public override async ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default)
        {
            var @lock = ValidateResourceNameThenPrepareWaitTimeThenGenerateLock(resourceName, lockExpiry, ref waitTimeToAcquireLock);
#pragma warning disable CS8629 // Nullable value type may be null.
            var handle = await @lock.TryAcquireAsync(timeout: waitTimeToAcquireLock.Value, cancellationToken: cancellationToken);
#pragma warning restore CS8629 // Nullable value type may be null.

            return handle is not null ? new DistributedSynchronizationHandle(handle) : null;
        }

        private RedisDistributedLock ValidateResourceNameThenPrepareWaitTimeThenGenerateLock(string resourceName, TimeSpan lockExpiry, ref TimeSpan? waitTimeToAcquireLock)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
                throw new ArgumentNullException(nameof(resourceName));

            waitTimeToAcquireLock ??= MedallionRedisLockOptions.DefaultWaitTimeToAcquireLock;

            return GenerateDistributeLockForAcquire(resourceName, lockExpiry);
        }

        private RedisDistributedLock GenerateDistributeLockForAcquire(string resourceName, TimeSpan lockExpiry)
        {
            return new RedisDistributedLock(resourceName, Options.Database
                            , options =>
                            {
                                options.Expiry(lockExpiry);
                            });
        }
    }
}
