using Daric.Database.Abstraction;
using Daric.Domain.Shared;

namespace Daric.Database.SqlServer;

public interface ISqlServerEventManager : IDbContextEventManager
{
    Task AddHandler(string key, ISqlServerEventHandler handler);
    Task Manage(ISqlServerEventManagerArg arg);

    Task<List<ISqlServerEventHandler>> GetHandlers(string key);
    Task<List<ISqlServerEventHandler>> GetHandlers<TEntity>() where TEntity : IEntity;

}
