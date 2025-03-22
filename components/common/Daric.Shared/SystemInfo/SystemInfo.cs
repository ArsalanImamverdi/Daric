using System.Net;

namespace Daric.Shared;

public class SystemInfo : ISystemInfo
{
    public IPAddress IPAddress { get; }

    public SystemInfo(IPAddress ipAddress)
    {
        IPAddress = ipAddress;
    }
}
