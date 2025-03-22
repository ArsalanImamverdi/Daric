using Microsoft.Extensions.DependencyInjection;

namespace Daric.Logging.Abstractions
{
    public interface ILoggingOptions
    {
    }
    public class LoggingOptions : ILoggingOptions
    {

    }

    public interface ILoggingOptionsBuilder
    {
        ILoggingOptions Build();
    }
    public class LoggingOptionsBuilder : ILoggingOptionsBuilder
    {
        private readonly ILoggingOptions _loggingOptions;
        public LoggingOptionsBuilder()
        {
            _loggingOptions = new LoggingOptions();
        }

        public ILoggingOptions Build()
        {
            return _loggingOptions;
        }
    }
}
