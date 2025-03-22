using Microsoft.Extensions.DependencyInjection;

namespace Daric.Shared;

public static class MicroserviceInfoExtensions
{
    public static IServiceCollection AddMicroserviceInfo(this IServiceCollection services, Func<MicroserviceInfoBuilder, MicroserviceInfoBuilder> builder)
    {
        services.AddSingleton<IMicroserviceInfo>(_ => builder(new MicroserviceInfoBuilder()).WithProcessId(Environment.ProcessId).Build());
        return services;
    }
}
