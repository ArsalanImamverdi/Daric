using Microsoft.Extensions.Configuration;

using Serilog;

namespace Daric.Logging.Abstractions
{
    public interface ILoggingBuilder : Microsoft.Extensions.Logging.ILoggingBuilder
    {
        LoggerConfiguration LoggerConfiguration { get; }
        IConfiguration Configuration { get; }
    }
}
