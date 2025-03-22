using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace Daric.Logging.Abstractions.Internals
{

    internal class DefaultLoggingBuilder(IServiceCollection services) : ILoggingBuilder
    {
        private readonly IServiceCollection _services = services;
        private readonly LoggerConfiguration _loggerConfiguration = new();
        private readonly IConfiguration _configuration = new ConfigurationBuilder().Build();

        public IServiceCollection Services => _services;

        public LoggerConfiguration LoggerConfiguration => _loggerConfiguration;

        public IConfiguration Configuration => _configuration;
    }
}
