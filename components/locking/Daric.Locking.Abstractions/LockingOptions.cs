using Microsoft.Extensions.DependencyInjection;

namespace Daric.Locking.Abstraction
{
    public class LockingOptions(IServiceCollection serviceCollection)
    {
        internal IServiceCollection ServiceCollection { get; } = serviceCollection;
    }
}
