using OpenTelemetry.Trace;

namespace Daric.Tracing.Zipkin;

internal class ZipkinTrace(Tracer tracer) : Abstraction.Trace(tracer)
{

}
