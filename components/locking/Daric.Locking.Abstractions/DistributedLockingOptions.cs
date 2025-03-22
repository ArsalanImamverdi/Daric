using Microsoft.Extensions.DependencyInjection;

namespace Daric.Locking.Abstraction
{
    public class DistributedLockingOptions(IServiceCollection serviceCollection)
    {
        public IServiceCollection ServiceCollection { get; } = serviceCollection;
    }
}
