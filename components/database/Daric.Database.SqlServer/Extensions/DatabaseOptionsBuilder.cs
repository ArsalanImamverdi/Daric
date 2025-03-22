using Daric.Database.Abstraction;
using Daric.Database.Internals.EntityHelpers;
using Daric.Database.SqlServer.Config;
using Daric.Database.SqlServer.Internals;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Daric.Database.SqlServer.Extensions;

public static class DatabaseOptionsBuilderExtensions
{
    public static DatabaseOptionsBuilder AddSqlDatabase(this DatabaseOptionsBuilder builder, Func<SqlDatabaseOptionsBuilder, SqlDatabaseOptionsBuilder> options)
    {
        builder.ServiceCollection.AddOptions<DefaultSqlServerDataConfig>("default").BindConfiguration("Config:DatabaseConfig");

        builder.ServiceCollection.AddSingleton<ISqlServerDataConfig>(srv =>
        {
            var optionsMonitor = srv.GetService<IOptionsMonitor<DefaultSqlServerDataConfig>>();
            var logger = srv.GetService<ILogger<DefaultSqlServerDataConfig>>();
            if (optionsMonitor is null)
            {
                logger?.LogWarning("Can not find any configuration for {name}, setting default value", typeof(ISqlServerDataConfig).Name);
            }
            var currentValue = optionsMonitor?.Get("default") ?? Activator.CreateInstance<DefaultSqlServerDataConfig>();
            optionsMonitor?.OnChange((opt, name) =>
            {
                if (name != "default")
                    return;
                Interlocked.Exchange(ref currentValue, opt);
            });
            return currentValue;
        });

        builder.ServiceCollection.AddOptions<TracingConfigContainer>().BindConfiguration("Config:Tracing:SqlTracingConfig");
        builder.ServiceCollection.AddSingleton<ISqlTracingConfig>(srv =>
        {
            var optionsMonitor = srv.GetService<IOptionsMonitor<TracingConfigContainer>>();
            var logger = srv.GetService<ILogger<TracingConfigContainer>>();
            if (optionsMonitor is null)
            {
                logger?.LogWarning("Can not find any configuration for {name}, setting default value", typeof(ISqlTracingConfig).Name);
            }
            var currentValue = optionsMonitor?.CurrentValue ?? Activator.CreateInstance<TracingConfigContainer>();
            optionsMonitor?.OnChange(opt => Interlocked.Exchange(ref currentValue, opt));
            return currentValue.SqlTracingConfig;
        });

        builder.ServiceCollection.AddScoped<DateTimeEntityPropertySet>();
        builder.ServiceCollection.AddSingleton<ISqlServerEventManager, DefaultSqlServerEventManager>();
        options(new SqlDatabaseOptionsBuilder(builder.ServiceCollection));

        return builder;
    }
}
