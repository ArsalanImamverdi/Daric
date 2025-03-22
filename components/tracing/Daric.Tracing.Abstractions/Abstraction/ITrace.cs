using OpenTelemetry.Trace;

namespace Daric.Tracing.Abstraction
{
    public interface ITrace
    {
        TelemetrySpan? StartSpan(string name);
        TelemetrySpan? StartSpan(string name, string? parentId);

        TelemetrySpan? StartRootSpan(string name);
        TelemetrySpan? StartActiveSpan(string name);
        TelemetrySpan? StartActiveSpan(string name, string? parentId);
    }
}
