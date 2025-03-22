using System.Net;

namespace Daric.Shared;

public interface ISystemInfo
{
    IPAddress IPAddress { get; }
}
