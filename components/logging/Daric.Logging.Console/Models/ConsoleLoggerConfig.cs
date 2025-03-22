using Daric.Logging.Internals;
using Serilog.Events;

namespace Daric.Logging.Console
{
    public class ConsoleLoggerConfig
    {
        public LogEventLevel LogLevel { get; set; }
        public bool? IncludeDateTime { get; set; }
        public LogLevelOverrideCollection? Overrides { get; set; }

    }
}
