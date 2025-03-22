using Microsoft.Extensions.DependencyInjection;

namespace Daric.Shared;

public static class AppInfoExtensions
{
    public static IServiceCollection AddAppInfo(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IAppInfo, AppInfo>();
        return serviceCollection;
    }
}
