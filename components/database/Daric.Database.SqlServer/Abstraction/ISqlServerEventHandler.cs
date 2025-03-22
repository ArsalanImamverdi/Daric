using Daric.Database.Abstraction;

namespace Daric.Database.SqlServer;

public interface ISqlServerEventHandler : IDbContextEventHandler<ISqlServerEventArgs>
{
    new Task Handle(ISqlServerEventArgs @event);
}

public interface ISqlServerEventHandler<TSqlServerEventArgs> : IDbContextEventHandler<TSqlServerEventArgs>
    where TSqlServerEventArgs: ISqlServerEventArgs
{
    new Task Handle(TSqlServerEventArgs @event);
}
