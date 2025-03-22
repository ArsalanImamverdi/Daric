using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.NetworkInformation;

namespace Daric.Shared;

public static class SystemInfoExtensions
{
    public static IServiceCollection AddSystemInfo(this IServiceCollection serviceCollection)
    {
        serviceCollection.TryAddSingleton<ISystemInfo>(serviceProvider => new SystemInfo(GetLocalIPv4(NetworkInterfaceType.Ethernet)));
        return serviceCollection;
    }

    private static IPAddress GetLocalIPv4(NetworkInterfaceType type)
    {
        IPAddress output = IPAddress.None;
        foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (item.NetworkInterfaceType == type && item.OperationalStatus == OperationalStatus.Up)
            {
                var adapterProperties = item.GetIPProperties();
                if (adapterProperties.GatewayAddresses.FirstOrDefault() != null)
                {
                    foreach (var ip in adapterProperties.UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork) continue;
                        output = ip.Address;
                        break;
                    }
                }
            }
            if (output != default) { break; }
        }
        return output!;
    }
}
