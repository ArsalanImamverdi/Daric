using System.Diagnostics;

using OpenTelemetry.Trace;

namespace Daric.Tracing.Abstraction.Extensions
{
    public static class TelemetrySpanExtensions
    {
        public static Activity? GetActivity(this TelemetrySpan telemetrySpan)
        {
            var activityField = typeof(TelemetrySpan).GetField("Activity", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return activityField?.GetValue(telemetrySpan) as Activity;
        }
        public static string? GetCurrentId(this TelemetrySpan telemetrySpan)
        {
            return telemetrySpan.GetActivity()?.Id;
        }

        public static string? GetParentId(this TelemetrySpan telemetrySpan)
        {
            return telemetrySpan.GetActivity()?.ParentId;
        }
    }
}
