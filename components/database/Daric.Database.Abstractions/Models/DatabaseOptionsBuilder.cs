using Microsoft.Extensions.DependencyInjection;

namespace Daric.Database.Abstraction
{
    public class DatabaseOptionsBuilder
    {
        public IServiceCollection ServiceCollection { get; }
        public DatabaseOptionsBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }
    }
}
