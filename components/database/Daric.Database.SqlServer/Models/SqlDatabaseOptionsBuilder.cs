using Daric.Database.SqlServer.Internals;

using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Daric.Database.SqlServer;

public class SqlDatabaseOptionsBuilder(IServiceCollection serviceCollection)
{
    public IServiceCollection ServiceCollection { get; } = serviceCollection;

    public SqlDatabaseOptionsBuilder WithConfig<TConfig>()
        where TConfig : class, ISqlServerDataConfig
    {
        ServiceCollection.AddOptions<TConfig>().Configure<IConfiguration>((opt, configuration) =>
        {
            opt.SqlServerConfig = configuration.GetSection("SqlServer").Get<SqlServerDataConfig>() ?? new SqlServerDataConfig();
        });
        ServiceCollection.AddSingleton<ISqlServerDataConfig>(sp =>
        {
            var monitor = sp.GetService<IOptionsMonitor<TConfig>>();
            var logger = sp.GetService<ILogger<TConfig>>();
            if (monitor is null)
            {
                logger?.LogWarning("Can not find any configuration for {name}, setting default value", typeof(TConfig).Name);
            }
            var currentValue = monitor?.CurrentValue ?? Activator.CreateInstance<TConfig>();
            monitor?.OnChange(opt => Interlocked.Exchange(ref currentValue, opt));
            return currentValue;
        });
        return this;
    }

    public SqlDatabaseOptionsBuilder AddContext<TContext>()
        where TContext : class, ISqlDbContext<TContext>
    {
        return AddContext<TContext>(opt => opt);
    }
    public SqlDatabaseOptionsBuilder AddContext<TContext>(Func<SqlServerRepositoriesOptionsBuilder<TContext>, SqlServerRepositoriesOptionsBuilder<TContext>> options)
        where TContext : class, ISqlDbContext<TContext>
    {
        ServiceCollection.AddScoped<ISqlServerSequenceProvider<TContext>, SequenceProvider<TContext>>();
        ServiceCollection.AddScoped<TContext>();
        options?.Invoke(new SqlServerRepositoriesOptionsBuilder<TContext>(ServiceCollection));

        return this;
    }

    public SqlDatabaseOptionsBuilder AddInterceptor<TInterceptor>() where TInterceptor : DbConnectionInterceptor
    {
        ServiceCollection.AddScoped<DbConnectionInterceptor, TInterceptor>();
        return this;
    }
}
