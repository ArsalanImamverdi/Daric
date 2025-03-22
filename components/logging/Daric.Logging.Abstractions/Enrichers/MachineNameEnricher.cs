using Serilog.Core;
using Serilog.Events;

namespace Daric.Logging.Abstractions.Enrichers;

public class MachineNameEnricher : ILogEventEnricher
{
    static LogEventProperty? machineNameProperty;
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var machineName = Environment.MachineName;
        machineNameProperty ??= propertyFactory.CreateProperty("MachineName", machineName);
        logEvent.AddPropertyIfAbsent(machineNameProperty);
    }
}
