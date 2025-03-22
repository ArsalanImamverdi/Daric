using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Daric.Database.SqlServer;

public abstract class DesignTimeDbContextFactory<TContext, TConfig> : IDesignTimeDbContextFactory<TContext>
  where TContext : SqlServerDbContext<TContext>
    where TConfig : class, ISqlServerDataConfig
{
    public TContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

        var config = configuration.GetSection("Config").Get<TConfig>();

        var context = (TContext)Activator.CreateInstance(typeof(TContext), config, null)!;
        context.DatabaseIsInDesignMode();
        return context;
    }
}
