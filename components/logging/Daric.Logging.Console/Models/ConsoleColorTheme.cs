using Serilog.Sinks.SystemConsole.Themes;

namespace Daric.Logging.Console.Models
{
    internal class ConsoleColorTheme
    {
        public static AnsiConsoleTheme Daric { get; } = new AnsiConsoleTheme(
           new Dictionary<ConsoleThemeStyle, string>
           {
               [ConsoleThemeStyle.Text] = "\x1b[38;5;0015m",
               [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;0007m",
               [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;0008m",
               [ConsoleThemeStyle.Invalid] = "\x1b[38;5;0011m",
               [ConsoleThemeStyle.Null] = "\x1b[38;5;0027m",
               [ConsoleThemeStyle.Name] = "\x1b[38;5;0007m",
               [ConsoleThemeStyle.String] = "\x1b[38;5;0045m",
               [ConsoleThemeStyle.Number] = "\x1b[38;5;0200m",
               [ConsoleThemeStyle.Boolean] = "\x1b[38;5;0027m",
               [ConsoleThemeStyle.Scalar] = "\x1b[38;5;0085m",
               [ConsoleThemeStyle.LevelVerbose] = "\x1b[38;5;6m",
               [ConsoleThemeStyle.LevelDebug] = "\x1b[38;5;0044m",
               [ConsoleThemeStyle.LevelInformation] = "\x1b[38;5;0015m",
               [ConsoleThemeStyle.LevelWarning] = "\x1b[38;5;0011m",
               [ConsoleThemeStyle.LevelError] = "\x1b[38;5;0015m\x1b[38;5;0009m",
               [ConsoleThemeStyle.LevelFatal] = "\x1b[38;5;0015m\x1b[48;5;0196m",
           });
    }
}
