using Daric.Database.Abstraction;
using Daric.Domain.Shared;

using Microsoft.EntityFrameworkCore;

namespace Daric.Database.SqlServer;

public interface ISqlServerEventArgs : IDbContextEventArgs
{
    IEntity Entity { get; }
    EntityState State { get; }
}
