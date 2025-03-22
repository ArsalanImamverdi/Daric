using Daric.Shared;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Daric.Tracing.Abstraction
{
    public static class TracingExtensions
    {
        public static IServiceCollection AddTracing(this IServiceCollection serviceCollection, Func<TracingOptions, TracingOptions> options)
        {
            serviceCollection.AddOpenTelemetry();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var microServiceInfo = serviceProvider.GetService<IMicroserviceInfo>();
            var appInfo = serviceProvider.GetService<IAppInfo>();

            options?.Invoke(new TracingOptions(serviceCollection, microServiceInfo?.Name, appInfo?.Version));

            return serviceCollection;
        }
    }

    public class TracingOptions(IServiceCollection serviceCollection, string? name, string? version)
    {
        public IServiceCollection ServiceCollection { get; } = serviceCollection;
        public string? Name { get; } = name;
        public string? Version { get; } = version;
    }
}
