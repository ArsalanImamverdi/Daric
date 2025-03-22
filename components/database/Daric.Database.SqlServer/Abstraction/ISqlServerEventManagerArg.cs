using Daric.Database.Abstraction;

namespace Daric.Database.SqlServer;

public interface ISqlServerEventManagerArg : IDbContextEventManagerArg
{
    IEnumerable<IEntityEntryItem> EntityEntryItems { get; }
}
