using Microsoft.Extensions.DependencyInjection;

namespace Daric.Database.Abstraction
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection, Func<DatabaseOptionsBuilder, DatabaseOptionsBuilder> builder)
        {
            builder(new DatabaseOptionsBuilder(serviceCollection));
            return serviceCollection;
        }
    }
}
