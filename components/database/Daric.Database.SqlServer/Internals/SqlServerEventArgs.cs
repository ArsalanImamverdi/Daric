using Daric.Domain.Shared;

using Microsoft.EntityFrameworkCore;

namespace Daric.Database.SqlServer.Internals;

internal class SqlServerEventArgs(IEntity entity, EntityState entityState) : ISqlServerEventArgs
{

    public IEntity Entity { get; } = entity;

    public EntityState State => entityState;
}
