namespace Daric.Database.SqlServer.Internals;

internal class SqlServerEventManagerArg(IEnumerable<IEntityEntryItem> entityEntryItems) : ISqlServerEventManagerArg
{
    public IEnumerable<IEntityEntryItem> EntityEntryItems { get; } = entityEntryItems;
}
