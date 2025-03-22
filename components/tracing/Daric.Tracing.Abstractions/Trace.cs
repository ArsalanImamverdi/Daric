using System.Diagnostics;

using Daric.Tracing.Abstraction.Extensions;

using OpenTelemetry.Trace;

namespace Daric.Tracing.Abstraction
{
    public class Trace(Tracer tracer) : ITrace
    {
        private readonly Tracer _tracer = tracer;

        public TelemetrySpan? StartActiveSpan(string name)
        {
            var span = _tracer.StartActiveSpan(name);
            return span;
        }

        public TelemetrySpan? StartActiveSpan(string name, string? parentId)
        {
            if (string.IsNullOrEmpty(parentId))
                return StartActiveSpan(name);

            var parentContext = ActivityContext.Parse(parentId, null);
            var span = _tracer.StartActiveSpan(name, SpanKind.Server, new SpanContext(parentContext.TraceId, parentContext.SpanId, ActivityTraceFlags.Recorded, isRemote: true));
            return span;
        }

        public TelemetrySpan? StartRootSpan(string name)
        {
            Activity.Current = null;
            var span = _tracer.StartRootSpan(name, SpanKind.Server);
            Activity.Current ??= span.GetActivity();
            return span;
        }

        public TelemetrySpan? StartSpan(string name)
        {
            var span = _tracer.StartSpan(name, SpanKind.Server);
            return span;
        }

        public TelemetrySpan? StartSpan(string name, string? parentId)
        {
            if (string.IsNullOrEmpty(parentId))
                return StartSpan(name);

            var parentContext = ActivityContext.Parse(parentId, null);
            return _tracer.StartSpan(name, SpanKind.Server, new SpanContext(parentContext.TraceId, parentContext.SpanId, ActivityTraceFlags.Recorded, isRemote: true));

        }
    }
}
