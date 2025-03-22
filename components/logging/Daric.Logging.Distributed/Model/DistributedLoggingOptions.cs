using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Serilog;
using Serilog.Core;

namespace Daric.Logging.Distributed.Model
{
    internal static class Constants
    {
        public const string DISTRIBUTED_LOGGER_CONFIG = "Logging:DistributedLoggerConfig";
    }
    public class DistributedLoggingOptions
    {
        public IDistributedLoggerConfig? Config { get; set; }
        public Type? ConfigType { get; set; }
        public Action<LoggerConfiguration>? LoggerConfiguration { get; set; }
        public Action<IDistributedLoggerConfig>? Override { get; set; }
        public List<Type>? Enrichers { get; set; }
        public string GetFromSection { get; set; } = Constants.DISTRIBUTED_LOGGER_CONFIG;
    }

    public class DistributedLoggingOptionsBuilder(DistributedLoggingOptions options)
    {
        public DistributedLoggingOptionsBuilder WithConfigFromSection(string section)
        {
            options.GetFromSection = section;
            return this;
        }
        public DistributedLoggingOptionsBuilder WithConfig<TConfig>(TConfig config) where TConfig : IDistributedLoggerConfig
        {
            options.Config = config;
            return this;
        }

        public DistributedLoggingOptionsBuilder ConfigLogging(Action<LoggerConfiguration> configuration)
        {
            options.LoggerConfiguration = configuration;
            return this;
        }

        public DistributedLoggingOptionsBuilder EnrichWith<TEnrich>() where TEnrich : class, ILogEventEnricher
        {
            options.Enrichers ??= [];
            options.Enrichers.Add(typeof(TEnrich));
            return this;
        }

        public DistributedLoggingOptionsBuilder WithDefaultConfigAndOverride(Action<IDistributedLoggerConfig> @override)
        {
            options.Override = @override;
            return this;
        }
    }


}
