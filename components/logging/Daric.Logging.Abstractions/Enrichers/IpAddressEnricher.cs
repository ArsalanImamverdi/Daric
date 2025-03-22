
using Daric.Logging.Abstractions.Internals;

using Serilog.Core;
using Serilog.Events;

namespace Daric.Logging.Abstractions.Enrichers;

public class IpAddressEnricher : ILogEventEnricher
{
    string? _ipAddress = null;

    private string GetIpAddresses()
    {
        return _ipAddress ??= IpAddressResolver.GetIpAddresses().Select(item => item.ToString()).Aggregate((f, s) => $"{f},{s}");
    }
    static LogEventProperty? ipAddressProperty;
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var ips = GetIpAddresses();
        ipAddressProperty ??= propertyFactory.CreateProperty("IpAddress", ips);
        logEvent.AddPropertyIfAbsent(ipAddressProperty);

    }
}
