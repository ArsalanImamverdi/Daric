using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Daric.Infrastructure.SqlServer
{
    internal class DaricDbContextDesignTimeFactory : IDesignTimeDbContextFactory<DaricDbContext>
    {
        DaricDbContext IDesignTimeDbContextFactory<DaricDbContext>.CreateDbContext(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(new ConfigurationBuilder().AddJsonFile("appsettings.json").AddJsonFile("appsettings.Development.json").Build());
            serviceCollection.AddLogging();
            serviceCollection.AddDaricDbContext();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider.CreateScope().ServiceProvider.GetRequiredService<DaricDbContext>();
        }
    }
}
