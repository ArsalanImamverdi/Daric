using System.Net;
using System.Net.NetworkInformation;

namespace Daric.Logging.Abstractions.Internals
{
    internal class IpAddressResolver
    {
        private static List<IPAddress>? _ips = null;

        private static List<IPAddress> GetIpAddressesInternal()
        {
            var result = new List<IPAddress>();
            try
            {
                NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                result = [.. (from NetworkInterface networkInterface in networkInterfaces
                          where networkInterface.OperationalStatus == OperationalStatus.Up 
                          let properties = networkInterface.GetIPProperties()
                          from UnicastIPAddressInformation address in properties.UnicastAddresses
                          where address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
                          select address.Address)];
            }
            catch
            {
            }
            return result;
        }

        public static List<IPAddress> GetIpAddresses() => _ips ??= GetIpAddressesInternal();
    }
}
