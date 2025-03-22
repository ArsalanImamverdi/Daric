namespace Daric.Database.SqlServer.Internals;

internal interface ISqlTracingConfig
{
    bool Enabled { get; set; }
    bool IncludeCommandText { get; set; }
}
internal class SqlTracingConfig : ISqlTracingConfig
{
    public bool Enabled { get; set; } = false;
    public bool IncludeCommandText { get; set; } = false;
}

internal class TracingConfigContainer
{
    public SqlTracingConfig SqlTracingConfig { get; set; } = new();
}
