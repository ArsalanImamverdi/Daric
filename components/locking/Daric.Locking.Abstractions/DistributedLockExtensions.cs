namespace Daric.Locking.Abstraction
{
    public static class DistributedLockExtensions
    {
        /// <summary>
        /// Check if the <paramref name="handle"/> is successfully acquired from the distributed source or not?
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static bool IsAcquired(this IDistributedSynchronizationHandle? handle)
        {
            return handle is not null;
        }
    }
}
