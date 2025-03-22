using Daric.Logging.Console.Models;
using Daric.Logging.Internals;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using System.Text;

namespace Daric.Logging.Console
{
    public static class ConsoleLoggerExtensions
    {
        public static Abstractions.ILoggingBuilder AddConsoleLogging(this Abstractions.ILoggingBuilder loggingBuilder, Func<ConsoleLoggingOptionsBuilder, ConsoleLoggingOptionsBuilder> options)
        {
            var name = Guid.NewGuid().ToString();
            loggingBuilder.Services.Configure<ConsoleLoggingOptions>(name, opt => options?.Invoke(new ConsoleLoggingOptionsBuilder(opt)));
            loggingBuilder.Services.AddOptions<DefaultConsoleLoggerConfig>(name)
                .PostConfigure<IServiceProvider>((opt, services) =>
                {
                    var configuration = services.GetRequiredService<IConfiguration>();
                    var option = services.GetRequiredService<IOptionsMonitor<ConsoleLoggingOptions>>().Get(name);
                    var section = configuration.GetSection(option?.GetFromSection ?? Constants.CONSOLE_LOGGER_CONFIG);

                    var config = option?.Config;
                    if (config is null)
                    {
                        config = new DefaultConsoleLoggerConfig() { ConsoleLoggerConfig = new() };

                        var includeDateTime = section.GetSection("IncludeDateTime");
                        config.ConsoleLoggerConfig!.IncludeDateTime = includeDateTime is null || includeDateTime.Get<bool>();

                        var logLevelSection = section.GetSection("LogLevel");
                        config.ConsoleLoggerConfig.LogLevel = logLevelSection.Value is null ? LogEventLevel.Information : logLevelSection.Get<LogEventLevel>();

                        var ov = section.GetSection("Overrides");
                        var child = ov.GetChildren().ToArray();
                        var logLevelOverrides = new LogLevelOverride[child.Length];
                        for (var i = 0; i < child.Length; i++)
                        {
                            logLevelOverrides[i] = new LogLevelOverride(child[i].Key, child[i].Get<LogEventLevel>());
                        }
                        config.ConsoleLoggerConfig.Overrides = new LogLevelOverrideCollection(logLevelOverrides);
                    }

                    config ??= new DefaultConsoleLoggerConfig() { ConsoleLoggerConfig = new() };

                    if (option?.Override is not null)
                        option.Override(config);

                    opt.ConsoleLoggerConfig = new ConsoleLoggerConfig()
                    {
                        IncludeDateTime = config.ConsoleLoggerConfig?.IncludeDateTime ?? true,
                        LogLevel = config.ConsoleLoggerConfig?.LogLevel ?? LogEventLevel.Information,
                        Overrides = config.ConsoleLoggerConfig?.Overrides ?? new LogLevelOverrideCollection([])
                    };
                });

            //loggingBuilder.Services.AddSingleton<IConsoleLoggerConfig>(srv => srv.GetRequiredService<IOptions<DefaultConsoleLoggerConfig>>().Value);
            loggingBuilder.Services.AddSingleton<LoggerConfiguration>(services =>
                {
                    var loggerConfig = new LoggerConfiguration();
                    var config = services.GetRequiredService<IOptionsMonitor<DefaultConsoleLoggerConfig>>().Get(name);
                    var options = services.GetRequiredService<IOptionsMonitor<ConsoleLoggingOptions>>().Get(name);
                    var template = new StringBuilder();
                    if (config.ConsoleLoggerConfig?.IncludeDateTime == true)
                        template.Append("{Timestamp:yyyy-MM-dd HH:mm:ss.fff} ");
                    template.Append("[{Level:u3}] {Verbose:lj}{Trace:lj}{Debug:lj}{Information:lj}{Warning:lj}{Error:lj}{Fatal:lj}{ExceptionDetail}{NewLine}");

                    loggerConfig.WriteTo
                      .Console(outputTemplate: template.ToString(), theme: ConsoleColorTheme.Daric)
                      .Enrich.FromLogContext()
                      .Enrich.With<ConsoleExceptionEnricher>()
                      .Enrich.With<ConolseColorEnricher>()
                        .Filter.With(new LogLevelFilter(config.ConsoleLoggerConfig!.LogLevel, config.ConsoleLoggerConfig.Overrides ?? new LogLevelOverrideCollection([])));

                    if (options?.LoggerConfiguration is not null)
                    {
                        options.LoggerConfiguration(loggerConfig);
                    }
                    return loggerConfig.MinimumLevel.Verbose();
                });

            return loggingBuilder;
        }
        public static Abstractions.ILoggingBuilder AddConsoleLogging(this Abstractions.ILoggingBuilder loggingBuilder)
        {
            return loggingBuilder.AddConsoleLogging(opt => opt);
        }
    }
}
