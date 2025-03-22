using Daric.Logging.Internals;
using Serilog.Events;
using System.Reflection;

namespace Daric.Logging.Distributed
{
    public enum Host
    {
        Elasticsearch = 0,
        OpenSearch = 1
    }
    public class DistributedLoggerConfig
    {
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
        public string[] Hosts { get; set; } = [];
        public string? IndexFormat { get; set; } = Assembly.GetExecutingAssembly().FullName;
        public Host HostType { get; set; } = Host.Elasticsearch;
        public bool IncludeDateInIndexFormat { get; set; } = true;
        public string? ApiKey { get; set; }
        public LogLevelOverrideCollection Overrides { get; set; } = new LogLevelOverrideCollection([]);
        public bool EnableErrorLog { get; set; } = true;
    }
}
