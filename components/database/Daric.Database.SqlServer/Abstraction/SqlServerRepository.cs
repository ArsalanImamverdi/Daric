using System.Linq.Expressions;

using Daric.Database.SqlServer.Internals;
using Daric.Domain.Shared;
using Daric.Tracing.Abstraction;
using Daric.Tracing.Abstraction.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Trace;

namespace Daric.Database.SqlServer;

public interface ISqlServerRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, IEntity
{
}
public abstract class SqlServerRepository<TEntity>(DbSet<TEntity> entity, IServiceProvider serviceProvider) : ISqlServerRepository<TEntity>
    where TEntity : class, IEntity
{
    private ISqlTracingConfig? _sqlTracingConfig;
    protected IServiceProvider ServiceProvider { get; } = serviceProvider;

    public DbSet<TEntity> Entity { get; } = entity;

    public Task BulkInsert(IEnumerable<TEntity> entities)
    {
        if (entities is null || entities.Any() == false)
            return Task.CompletedTask;

        var span = GetSpan("BulkInsert");
        var traceId = span?.GetCurrentId();

        var result = Entity.AddRangeAsync(entities);
        SetTraceId(Entity.Entry(entities.FirstOrDefault()!), traceId);
        span?.End();
        return result;
    }

    public Task BulkUpdate(IEnumerable<TEntity> entities)
    {
        if (entities is null || !entities.Any())
            return Task.CompletedTask;

        var span = GetSpan("BulkUpdate");
        var traceId = span?.GetCurrentId();

        Entity.UpdateRange(entities);

        SetTraceId(Entity.Entry(entities.FirstOrDefault()!), traceId);
        span?.End();
        return Task.CompletedTask;
    }

    public Task Delete(TEntity entity)
    {
        var span = GetSpan("Delete");
        var traceId = span?.GetCurrentId();
        EntityEntry<TEntity>? entry;
        if (entity is IRemovableEntity entityAsSoftDelete)
        {
            entityAsSoftDelete.IsDeleted = true;
            entry = Entity.Entry(entity);
        }
        else
        {
            entry = Entity.Remove(entity);
        }

        SetTraceId(entry, traceId);
        span?.End();
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<TEntity>> FilterAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var span = GetSpan("Filter");

        var result = await Entity.Where(predicate).ToListAsync();

        span?.End();

        return result;
    }

    public async Task Insert(TEntity entity)
    {
        var span = GetSpan("Insert");
        var traceId = span?.GetCurrentId();
        var add = await Entity.AddAsync(entity);
        //var command = RelationalDependencies.BatchPreparer.BatchCommands
        SetTraceId(add, traceId);
        span?.End();
    }

    public Task Update(TEntity entity)
    {
        var span = GetSpan("Update");
        var traceId = span?.GetCurrentId();
        var update = Entity.Update(entity);
        SetTraceId(update, traceId);
        span?.End();
        return Task.CompletedTask;
    }

    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var span = GetSpan("FirstOrDefault");
        var result = await Entity.FirstOrDefaultAsync(predicate);
        span?.End();
        return result;
    }

    protected TelemetrySpan? GetSpan(string name)
    {
        _sqlTracingConfig ??= ServiceProvider.GetService<ISqlTracingConfig>();
        if (_sqlTracingConfig?.Enabled == false)
            return null;

        var trace = ServiceProvider.GetService<ITrace>();
        var span = trace?.StartActiveSpan($"SqlServer: {name}");
        return span;
    }
    protected void SetTraceId(EntityEntry<TEntity>? entity, string? traceId)
    {
        if (entity is null)
            return;

        entity.Metadata.SetRuntimeAnnotation("traceId", traceId);
    }

    public async Task<TEntity?> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var span = GetSpan("FirstOrDefault");
        var result = await Entity.LastOrDefaultAsync(predicate);
        span?.End();
        return result;
    }
}
