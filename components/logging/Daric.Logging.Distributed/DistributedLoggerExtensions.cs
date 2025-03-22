using System.Reflection;

using Daric.Logging.Abstractions.Enrichers;
using Daric.Logging.Distributed.Model;
using Daric.Logging.Internals;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

using Constants = Daric.Logging.Distributed.Model.Constants;

namespace Daric.Logging.Distributed
{
    public static class DistributedLoggerExtensions
    {
        public static Abstractions.ILoggingBuilder AddDistributedLogging(this Abstractions.ILoggingBuilder loggingBuilder, Func<DistributedLoggingOptionsBuilder, DistributedLoggingOptionsBuilder> options)
        {
            var name = Guid.NewGuid().ToString();
            loggingBuilder.Services.Configure<DistributedLoggingOptions>(name, opt => options?.Invoke(new DistributedLoggingOptionsBuilder(opt)));
            loggingBuilder.Services.AddOptions<DefaultDistributedLoggerConfig>(name)
                .Configure<IServiceProvider>((opt, services) =>
                {
                    var configuration = services.GetRequiredService<IConfiguration>();
                    var option = services.GetRequiredService<IOptionsMonitor<DistributedLoggingOptions>>().Get(name);
                    var section = configuration.GetSection(option?.GetFromSection ?? Constants.DISTRIBUTED_LOGGER_CONFIG);

                    var config = option?.Config;
                    if (config is null)
                    {
                        config = new DefaultDistributedLoggerConfig() { DistributedLoggerConfig = new DistributedLoggerConfig() };

                        config.DistributedLoggerConfig!.ApiKey = section.GetSection("ApiKey").Value;

                        var enableErrorLogSection = section.GetSection("EnableErrorLog");
                        config.DistributedLoggerConfig.EnableErrorLog = enableErrorLogSection.Value is null || enableErrorLogSection.Get<bool>();

                        config.DistributedLoggerConfig.Hosts = section.GetSection("Hosts").Get<string[]>() ?? [];
                        config.DistributedLoggerConfig.HostType = section.GetSection("HostType").Get<Host>();

                        var includeDateInIndexFormatSection = section.GetSection("IncludeDateInIndexFormat");
                        config.DistributedLoggerConfig.IncludeDateInIndexFormat = includeDateInIndexFormatSection.Value is null || includeDateInIndexFormatSection.Get<bool>();

                        var indexFormatSection = section.GetSection("IndexFormat");
                        config.DistributedLoggerConfig.IndexFormat = indexFormatSection is null ? Assembly.GetExecutingAssembly().FullName : indexFormatSection.Get<string>();

                        var logLevelSection = section.GetSection("LogLevel");
                        config.DistributedLoggerConfig.LogLevel = logLevelSection.Value is null ? LogEventLevel.Information : logLevelSection.Get<LogEventLevel>();

                        var ov = section.GetSection("Overrides");
                        var child = ov.GetChildren().ToArray();
                        var logLevelOverrides = new LogLevelOverride[child.Length];
                        for (var i = 0; i < child.Length; i++)
                        {
                            logLevelOverrides[i] = new LogLevelOverride(child[i].Key, child[i].Get<LogEventLevel>());
                        }
                        config.DistributedLoggerConfig.Overrides = new LogLevelOverrideCollection(logLevelOverrides);
                    }

                    config ??= new DefaultDistributedLoggerConfig() { DistributedLoggerConfig = new() };

                    if (option?.Override is not null)
                        option.Override(config);

                    opt.DistributedLoggerConfig = new DistributedLoggerConfig()
                    {
                        ApiKey = config.DistributedLoggerConfig!.ApiKey,
                        EnableErrorLog = config.DistributedLoggerConfig.EnableErrorLog,
                        Hosts = config.DistributedLoggerConfig.Hosts,
                        HostType = config.DistributedLoggerConfig.HostType,
                        IncludeDateInIndexFormat = config.DistributedLoggerConfig.IncludeDateInIndexFormat,
                        IndexFormat = config.DistributedLoggerConfig.IndexFormat,
                        LogLevel = config.DistributedLoggerConfig.LogLevel,
                        Overrides = config.DistributedLoggerConfig.Overrides

                    };
                });
            loggingBuilder.Services.AddSingleton<LoggerConfiguration>(services =>
                                    {
                                        var loggerConfig = new LoggerConfiguration();
                                        var opt = services.GetRequiredService<IOptionsMonitor<DistributedLoggingOptions>>().Get(name);
                                        var config = services.GetRequiredService<IOptionsMonitor<DefaultDistributedLoggerConfig>>().Get(name);

                                        if (config?.DistributedLoggerConfig is null)
                                            return loggerConfig;

                                        if (string.IsNullOrEmpty(config?.DistributedLoggerConfig?.IndexFormat))
                                            return loggerConfig;

                                        config.DistributedLoggerConfig.IndexFormat = config.DistributedLoggerConfig.IndexFormat.Replace('.', '-');

                                        config.DistributedLoggerConfig.IndexFormat = config.DistributedLoggerConfig.IndexFormat.ToLower()/*.Replace("logs-", "")*/;
                                        var dataFormat = "{0:yyyy-MM-dd}";
                                        if (config.DistributedLoggerConfig.IncludeDateInIndexFormat && !config.DistributedLoggerConfig.IndexFormat.EndsWith(dataFormat))
                                        {
                                            config.DistributedLoggerConfig.IndexFormat = $"{config.DistributedLoggerConfig.IndexFormat}-{dataFormat}";
                                        }
                                        var hosts = config.DistributedLoggerConfig?.Hosts.Select(server =>
                                        {
                                            var validHost = Uri.TryCreate(server, UriKind.Absolute, out var uri);
                                            return !validHost ? default : uri;
                                        }).Where(uri => uri is not null).ToArray() ?? [];

                                        if (hosts.Length == 0)
                                            throw new InvalidOperationException("Invalid DistributedLogging hosts");


                                        var logLevelFilter = new LogLevelFilter(config.DistributedLoggerConfig!.LogLevel, config.DistributedLoggerConfig.Overrides);
                                        var elasticSearchOptions = new ElasticsearchSinkOptions(hosts)
                                        {
                                            IndexFormat = config.DistributedLoggerConfig!.IndexFormat,
                                            MinimumLogEventLevel = LogEventLevel.Verbose,
                                            AutoRegisterTemplate = false,
                                            BatchAction = ElasticOpType.Create,
                                            BatchPostingLimit = 100,
                                            QueueSizeLimit = 500
                                        };

                                        if (!string.IsNullOrEmpty(config.DistributedLoggerConfig?.ApiKey))
                                        {
                                            elasticSearchOptions.ModifyConnectionSettings = connection => connection.GlobalHeaders(
                                                new()
                                                {
                                                    {
                                                        "Authorization", $"ApiKey {config.DistributedLoggerConfig?.ApiKey}"
                                                    }
                                                });
                                        }

                                        if (config.DistributedLoggerConfig?.EnableErrorLog == true)
                                        {
                                            elasticSearchOptions.FailureCallback = (logEvent) =>
                                            {
                                                if (logEvent.Exception is not null)
                                                {
                                                    var logger = services.GetService<ILogger<ElasticsearchSink>>();
                                                    logger?.LogError(logEvent.Exception, nameof(ElasticsearchSink));
                                                }
                                            };
                                            elasticSearchOptions.EmitEventFailure = EmitEventFailureHandling.RaiseCallback;
                                        }

                                        loggerConfig.WriteTo.Async(@async => @async.Elasticsearch(elasticSearchOptions)).Enrich.FromLogContext()
                                             .Enrich.With<MachineNameEnricher>()
                                             .Enrich.With<IpAddressEnricher>()
                                             .Enrich.With<TraceIdEnricher>()
                                             .Filter.ByExcluding(Matching.WithProperty("SourceContext", "Serilog.Sinks.Elasticsearch.ElasticsearchSink"))
                                             .Filter.With(logLevelFilter);

                                        if (opt?.Enrichers?.Count > 0)
                                        {
                                            var enrichers = opt.Enrichers
                                                            .Select(enricherType => Activator.CreateInstance(enricherType))
                                                            .Where(enricher => enricher is not null)
                                                            .ToArray() as ILogEventEnricher[];

                                            if (enrichers is not null && enrichers.Length > 0)
                                                loggerConfig.Enrich.With(enrichers);
                                        }

                                        opt?.LoggerConfiguration?.Invoke(loggerConfig);
                                        return loggerConfig.MinimumLevel.Verbose();
                                    });

            return loggingBuilder;
        }
        public static Abstractions.ILoggingBuilder AddDistributedLogging(this Abstractions.ILoggingBuilder loggingBuilder)
        {
            return loggingBuilder.AddDistributedLogging(opt => opt);
        }
    }
}
