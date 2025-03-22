using Serilog.Core;
using Serilog.Events;

namespace Daric.Logging.Console.Models
{
    internal class ConsoleExceptionEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (logEvent.Exception is not null)
            {
                var prop = propertyFactory.CreateProperty("ExceptionDetail", $" :\x1b[38;5;0009m {logEvent.Exception.Message}{(logEvent.Exception.InnerException is not null ? $"({logEvent.Exception.InnerException.Message})" : "")}");
                logEvent.AddPropertyIfAbsent(prop);
                return;
            }
        }
    }
    internal class ConolseColorEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            string color = logEvent.Level switch
            {
                LogEventLevel.Verbose => "\x1b[38;5;6m",
                LogEventLevel.Debug => "\x1b[38;5;0044m",
                LogEventLevel.Warning => "\x1b[38;5;0011m",
                LogEventLevel.Error => "\x1b[38;5;0009m",
                LogEventLevel.Fatal => "\x1b[48;5;0196m",
                _ => "\x1b[38;5;0015m",
            };
            var prop = propertyFactory.CreateProperty(logEvent.Level.ToString(), $"{color}{logEvent.RenderMessage()}");
            logEvent.AddPropertyIfAbsent(prop);
        }
    }
}
