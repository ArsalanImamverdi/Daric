using Daric.Logging.Abstractions;
using Daric.Logging.Abstractions.Internals;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

namespace Daric.Logging.Abstractions
{
    public static class LoggerExtensions
    {
        public static IServiceCollection AddLogging(this IServiceCollection serviceCollection, Func<ILoggingBuilder, ILoggingBuilder> options)
        {
            serviceCollection.AddLogging(x =>
            {
                x.ClearProviders();
            });
            options?.Invoke(new DefaultLoggingBuilder(serviceCollection));
            serviceCollection.AddSerilog((services, serilogOptions) =>
            {
                var loggerConfigurations = services.GetServices<LoggerConfiguration>();
                foreach (var loggerConfiguration in loggerConfigurations)
                {
                    var logger = loggerConfiguration.CreateLogger();
                    serilogOptions.WriteTo.Logger(logger);
                    serilogOptions.MinimumLevel.Verbose();
                }

            });

            return serviceCollection;
        }
    }
}
