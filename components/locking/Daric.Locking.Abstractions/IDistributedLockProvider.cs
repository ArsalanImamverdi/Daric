using Microsoft.Extensions.DependencyInjection;

namespace Daric.Locking.Abstraction
{
    public interface IDistributedLockProvider
    {
        IDistributedLockMechanism? Get<TDistributedLockMechanism>() where TDistributedLockMechanism : IDistributedLockMechanism;
    }

    public class DefaultDistributedLockProvider(IServiceProvider serviceProvider) : IDistributedLockProvider
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IDistributedLockMechanism? Get<TDistributedLockMechanism>() where TDistributedLockMechanism : IDistributedLockMechanism
        {
            return _serviceProvider.GetService<TDistributedLockMechanism>();
        }
        public IDistributedLockMechanism? Get()
        {
            return _serviceProvider.GetService<IDistributedLockMechanism>();
        }
    }
}
