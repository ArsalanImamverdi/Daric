

namespace Daric.Locking.Abstraction;

public abstract class BaseDistributedLockMechanism<TOptions>(string providerName, TOptions options) : IDistributedLockMechanism<TOptions>
{

    public string ProviderName { get; } = providerName;
    public TOptions Options { get; } = options;
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IsDistributedProvider => true;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract bool IsLockExpirySupport { get; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract IDistributedSynchronizationHandle Acquire(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract ValueTask<IDistributedSynchronizationHandle> AcquireAsync(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract IDistributedSynchronizationHandle? TryAcquire(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public abstract ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);
}
