using Microsoft.Extensions.DependencyInjection;

namespace Daric.Locking.Abstraction
{
    public static class LockingOptionsExtensions
    {
        public static IServiceCollection AddDistributedLock(this IServiceCollection serviceCollection, Func<DistributedLockingOptions, DistributedLockingOptions> options)
        {
            options?.Invoke(new DistributedLockingOptions(serviceCollection));
            return serviceCollection;
        }
    }
}
