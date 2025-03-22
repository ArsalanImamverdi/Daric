
namespace Daric.Locking.Abstraction
{
    /// <summary>
    /// Distributed lock provider operations
    /// </summary>
    public interface IDistributedLockMechanism 
    {
        /// <summary>
        /// Acquires the lock synchronously, failing with <see cref="TimeoutException"/> if the attempt times out specified by <paramref name="waitTimeToAcquireLock"/>.
        /// Usage: 
        /// <code>
        ///     using (myLockProvider.Acquire(...))
        ///     {
        ///         /* we have the lock! let's do the business */
        ///     }
        ///     // dispose releases the lock
        /// </code>
        /// </summary>
        /// <param name="resourceName">Acquire a lock with this name</param>
        /// <param name="lockExpiry">How long the lock will be last</param>
        /// <param name="waitTimeToAcquireLock">How long to wait before giving up on the acquisition attempt. Defaults set in each provider's options</param>
        /// <param name="cancellationToken">Specifies a token by which the wait can be canceled. Allows the operation to be interrupted via cancellation. Note that this won't cancel the hold on the lock once the acquire succeeds.</param>
        /// <returns>If the lock acuqired successfully, a disposable handle to the lock returned else returns null</returns>
        /// <exception cref="TimeoutException"> raises if the attempt times out specified by <paramref name="waitTimeToAcquireLock"/></exception>
        IDistributedSynchronizationHandle Acquire(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Acquires the lock asynchronously, failing with <see cref="TimeoutException"/> if the attempt times out specified by <paramref name="waitTimeToAcquireLock"/>.
        /// Usage: 
        /// <code>
        ///     await using (await myLockProvider.AcquireAsync(...))
        ///     {
        ///         /* we have the lock! let's do the business */
        ///     }
        ///     // dispose releases the lock
        /// </code>
        /// </summary>
        /// <param name="resourceName">Acquire a lock with this name</param>
        /// <param name="lockExpiry">How long the lock will be last</param>
        /// <param name="waitTimeToAcquireLock">How long to wait before giving up on the acquisition attempt. Defaults set in each provider's options</param>
        /// <param name="cancellationToken">Specifies a token by which the wait can be canceled. Allows the operation to be interrupted via cancellation. Note that this won't cancel the hold on the lock once the acquire succeeds.</param>
        /// <returns>If the lock acuqired successfully, a disposable handle to the lock returned else returns null</returns>
        /// <exception cref="TimeoutException"> raises if the attempt times out specified by <paramref name="waitTimeToAcquireLock"/></exception>
        ValueTask<IDistributedSynchronizationHandle> AcquireAsync(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to acquire the lock synchronously. Usage: 
        /// <code>
        ///     using (var handle = myLockProvider.TryAcquire(...))
        ///     {
        ///         if (handle.IsAcquired()) 
        ///         {
        ///             /* we have the lock! let's do the business */ 
        ///         }
        ///     }
        ///     // dispose releases the lock if we took it
        /// </code>
        /// </summary>
        /// <param name="resourceName">Acquire a lock with this name</param>
        /// <param name="lockExpiry">How long the lock will be last</param>
        /// <param name="waitTimeToAcquireLock">How long to wait before giving up on the acquisition attempt. Defaults set in each provider's options</param>
        /// <param name="cancellationToken">Specifies a token by which the wait can be canceled. Allows the operation to be interrupted via cancellation. Note that this won't cancel the hold on the lock once the acquire succeeds.</param>
        /// <returns>If the lock acuqired successfully, a disposable handle to the lock returned else returns null</returns>
        IDistributedSynchronizationHandle? TryAcquire(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to acquire the lock asynchronously. Usage: 
        /// <code>
        ///     await using (var handle = await myLockProvider.TryAcquireAsync(...))
        ///     {
        ///         if (handle.IsAcquired()) 
        ///         {
        ///             /* we have the lock! let's do the business */ 
        ///         }
        ///     }
        ///     // dispose releases the lock if we took it
        /// </code>
        /// </summary>
        /// <param name="resourceName">Acquire a lock with this name</param>
        /// <param name="lockExpiry">How long the lock will be last</param>
        /// <param name="waitTimeToAcquireLock">How long to wait before giving up on the acquisition attempt. Defaults set in each provider's options</param>
        /// <param name="cancellationToken">Specifies a token by which the wait can be canceled. Allows the operation to be interrupted via cancellation. Note that this won't cancel the hold on the lock once the acquire succeeds.</param>
        /// <returns>If the lock acuqired successfully, a disposable handle to the lock returned else returns null</returns>
        ValueTask<IDistributedSynchronizationHandle?> TryAcquireAsync(string resourceName, TimeSpan lockExpiry, TimeSpan? waitTimeToAcquireLock = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Does the provider support LockExpiry (TTL) for auto-expiring the lock?
        /// </summary>
        bool IsLockExpirySupport { get; }
    }

    /// <summary>
    /// Distributed lock provider operations (Generic version)
    /// </summary>
    public interface IDistributedLockMechanism<TOptions> : IDistributedLockMechanism
    {

    }
}
