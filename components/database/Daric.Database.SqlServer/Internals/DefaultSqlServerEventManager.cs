using Daric.Database.Abstraction;
using Daric.Domain.Shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Daric.Database.SqlServer.Internals;

internal class DefaultSqlServerEventManager : ISqlServerEventManager
{
    private readonly Dictionary<string, List<ISqlServerEventHandler>> _handlers = [];
    public Task AddHandler(string key, ISqlServerEventHandler handler)
    {
        if (_handlers.TryGetValue(key, out var handlers))
        {
            if (handlers.Any(item => item.Type == handler.Type))
                return Task.CompletedTask;
            handlers.Add(handler);
        }
        else
        {
            handlers = [handler];
            _handlers.Add(key, handlers);
        }

        return Task.CompletedTask;
    }

    public Task<List<ISqlServerEventHandler>> GetHandlers(string key)
    {
        return Task.FromResult(_handlers.TryGetValue(key, out var handlers) ? handlers : []);
    }

    public Task<List<ISqlServerEventHandler>> GetHandlers<TEntity>() where TEntity : IEntity
    {
        return Task.FromResult(_handlers.TryGetValue(typeof(TEntity).FullName ?? string.Empty, out var handlers) ? handlers : []);
    }

    public async Task Manage(ISqlServerEventManagerArg arg)
    {
        foreach (var entity in arg.EntityEntryItems)
        {
            var entityName = entity.EntityEntry.Metadata.ClrType.FullName;
            if (string.IsNullOrEmpty(entityName))
                continue;

            var hasHandler = _handlers.TryGetValue(entityName, out var handlers);
            if (!hasHandler || handlers is null || handlers is { Count: 0 }) continue;
            switch (entity.State)
            {
                case EntityState.Deleted:
                    var deletedHandlers = handlers.Where(item => item.Type is EventType.OnDelete or EventType.All);
                    foreach (var deletedHandler in deletedHandlers)
                    {
                        await deletedHandler.Handle(new SqlServerEventArgs((IEntity)entity.EntityEntry.Entity, entity.State));
                    }
                    break;
                case EntityState.Modified:
                    var updatedHandlers = handlers.Where(item => item.Type is EventType.OnUpdate or EventType.All);
                    foreach (var updatedHandler in updatedHandlers)
                    {
                        await updatedHandler.Handle(new SqlServerEventArgs((IEntity)entity.EntityEntry.Entity, entity.State));
                    }
                    break;
                case EntityState.Added:
                    var insertHandlers = handlers.Where(item => item.Type is EventType.OnInsert or EventType.All);
                    foreach (var insertHandler in insertHandlers)
                    {
                        await insertHandler.Handle(new SqlServerEventArgs((IEntity)entity.EntityEntry.Entity, entity.State));
                    }
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    continue;
            }
        }
    }

    Task IDbContextEventManager.AddHandler(string key, IDbContextEventHandler handler)
    {
        return AddHandler(key, (ISqlServerEventHandler)handler);
    }

    Task IDbContextEventManager.Manage(IDbContextEventManagerArg arg)
    {
        return Manage((ISqlServerEventManagerArg)arg);
    }
}
