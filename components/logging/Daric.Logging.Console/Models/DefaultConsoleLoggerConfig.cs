using Daric.Logging.Abstractions;

namespace Daric.Logging.Console.Models
{
    public class DefaultConsoleLoggerConfig : IConsoleLoggerConfig
    {
        public ConsoleLoggerConfig? ConsoleLoggerConfig { get; set; }
    }
}
