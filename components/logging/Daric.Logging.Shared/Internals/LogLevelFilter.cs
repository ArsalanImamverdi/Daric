using System.Text;

using Serilog.Core;
using Serilog.Events;

namespace Daric.Logging.Internals;

public record LogLevelOverrideCollection(LogLevelOverride[] Overrides)
{
    public LogLevelOverrideCollection() : this([])
    {

    }
}
public record LogLevelOverride(string Key, LogEventLevel Level);

internal class LogLevelFilter(LogEventLevel globalLogLevel, LogLevelOverrideCollection overrides) : ILogEventFilter
{
    const string SOURECE_CONTEXT = "SourceContext";
    const char DOUBLE_QUOTE = '"';
    public LogEventLevel GlobalLogLevel { get; } = globalLogLevel;
    public LogLevelOverrideCollection Overrides { get; } = overrides;

    public bool IsEnabled(LogEvent logEvent)
    {
        if (Overrides.Overrides.Length == 0)
            return logEvent.Level >= GlobalLogLevel;

        var logLevelValue = GetLogEventLevel(logEvent);
        return logLevelValue switch
        {
            not null => logEvent.Level >= logLevelValue.Value,
            _ => logEvent.Level >= GlobalLogLevel
        };
    }

    LogEventLevel? GetLogEventLevel(LogEvent logEvent)
    {
        if (!logEvent.Properties.TryGetValue(SOURECE_CONTEXT, out var logProp))
            return null;
        using var textWriter = new StringWriter();
        logProp.Render(textWriter);

        var span = textWriter.GetStringBuilder();

        if (span.Length < 2)
            return null;
        var startIndex = 0;
        var endIndex = span.Length;
        if (span[0] == DOUBLE_QUOTE)
            startIndex++;
        if (span[^1] == DOUBLE_QUOTE)
            endIndex--;

        foreach (var keyValue in Overrides.Overrides ?? [])
        {
            if (KeyMatches(span, keyValue.Key, startIndex, endIndex))
            {
                return keyValue.Level;
            }
        }
        return null;
    }
    bool KeyMatches(StringBuilder value, ReadOnlySpan<char> key, int startIndex, int endIndex)
    {
        if (key.Length > value.Length)
            return false;

        for (int i = startIndex; i < endIndex; i++)
        {
            if (i - startIndex >= key.Length)
                break;
            if (char.ToLowerInvariant(value[i]) != char.ToLowerInvariant(key[i - startIndex]))
            {
                return false;
            }
        }

        return true;
    }
}