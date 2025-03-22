using Serilog;

namespace Daric.Logging.Console
{
    internal static class Constants
    {
        public const string CONSOLE_LOGGER_CONFIG = "Logging:ConsoleLoggerConfig";
    }
    public class ConsoleLoggingOptions
    {
        public IConsoleLoggerConfig? Config { get; set; }
        public Type? ConfigType { get; set; }
        public Action<LoggerConfiguration>? LoggerConfiguration { get; set; }
        public string GetFromSection { get; set; } = Constants.CONSOLE_LOGGER_CONFIG;

        public Action<IConsoleLoggerConfig>? Override { get; set; }

    }

    public class ConsoleLoggingOptionsBuilder(ConsoleLoggingOptions opt)
    {
        private readonly ConsoleLoggingOptions _options = opt;

        public ConsoleLoggingOptionsBuilder WithConfig<TConfig>(TConfig config) where TConfig : IConsoleLoggerConfig
        {
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            _options.Config = config;
            return this;
        }

        public ConsoleLoggingOptionsBuilder WithConfigFromSection(string section)
        {
            _options.GetFromSection = section;
            return this;
        }

        public ConsoleLoggingOptionsBuilder ConfigLogging(Action<LoggerConfiguration> configuration)
        {
            _options.LoggerConfiguration = configuration;
            return this;
        }

        public ConsoleLoggingOptionsBuilder WithDefaultConfigAndOverride(Action<IConsoleLoggerConfig> @override)
        {
            _options.Override = @override;
            return this;
        }
    }
}
