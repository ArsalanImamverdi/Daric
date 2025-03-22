using System.Net;

using Daric.Configurations;

namespace Daric.HttpApi.Utilities
{
    internal static class WebApplicationExtensions
    {
        public static Task RunAppAsync(this WebApplication webApplication)
        {
            webApplication.Urls.Clear();
            var scopedServiceProvider = webApplication.Services.CreateScope().ServiceProvider;
            var config = scopedServiceProvider.GetRequiredService<IConfig>();
            var logger = webApplication.Services.GetService<ILogger<WebApplication>>();
            var serverPorts = config.ServerPorts;
            if (serverPorts is null || serverPorts.Count == 0)
            {
                throw new InvalidOperationException("Can not find any ports on app settings file!");
            }
            serverPorts.ForEach(port =>
            {
                var address = $"http://{IPAddress.Any}:{port}";
                webApplication.Urls.Add(address);
            });
            return webApplication.RunAsync();
        }
    }
}
