using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Daric.Logging.Abstractions.Enrichers;

public class TraceIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Activity.Current?.TraceId;
        if (traceId is not null)
        {
            var traceIdProperty = propertyFactory.CreateProperty("TraceId", traceId);
            logEvent.AddPropertyIfAbsent(traceIdProperty);
        }

        var spanId = Activity.Current?.SpanId;
        if (spanId is not null)
        {
            var spanIdProperty = propertyFactory.CreateProperty("SpanId", spanId);
            logEvent.AddPropertyIfAbsent(spanIdProperty);
        }
    }
}
