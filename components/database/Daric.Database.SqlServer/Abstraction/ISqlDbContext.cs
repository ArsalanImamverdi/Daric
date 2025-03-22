using Daric.Database.Abstraction;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Daric.Database.SqlServer;

internal interface IInterceptedDbContext
{
    void AddInterceptor(IInterceptor interceptor);
}
public interface ISqlDbContext<out TContext> : IDbContext where TContext : ISqlDbContext<TContext>
{
    ILogger<TContext> Logger { get; }
    ISqlServerDataConfig Config { get; }
    DatabaseFacade Database { get; }
    ChangeTracker ChangeTracker { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

}
