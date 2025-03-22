using System.Data;
using System.Data.Common;

using Daric.Database.Internals.EntityHelpers;
using Daric.Database.SqlServer.Internals;
using Daric.Domain.Shared;
using Daric.Tracing.Abstraction;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using OpenTelemetry.Trace;

namespace Daric.Database.SqlServer;

public class SqlServerUnitOfWork<TContext>(TContext context, IDomainEventDispatcher domainEventDispatcher, IServiceProvider serviceProvider) : IUnitOfWork
    where TContext : ISqlDbContext<TContext>
{
    public TContext Context { get; } = context;
    private IDbContextTransaction? _currentTransaction = null;
    private TelemetrySpan? _currentTrace;
    private ISqlTracingConfig? _sqlTracingConfig;

    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public IDbContextTransaction? CurrentTransaction => _currentTransaction;


    private ITrace? _trace;
    private ITrace? GetTrace()
    {
        _sqlTracingConfig ??= ServiceProvider.GetService<ISqlTracingConfig?>();
        if (_sqlTracingConfig?.Enabled == false)
            return null;

        return _trace ??= ServiceProvider?.GetService<ITrace>();
    }
    public async Task BeginTransactionAsync()
    {
        await BeginTransactionAsync(CancellationToken.None);
    }
    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        _currentTransaction = await Context.Database.BeginTransactionAsync(cancellationToken);
        var trace = GetTrace();
        _currentTrace = trace?.StartActiveSpan("SqlServer: BeginTransaction");
    }
    public async Task UseTransactionAsync(DbTransaction transaction)
    {
        await UseTransactionAsync(transaction, CancellationToken.None);
    }
    public async Task UseTransactionAsync(DbTransaction transaction, CancellationToken cancellationToken)
    {
        _currentTransaction = await Context.Database.UseTransactionAsync(transaction, cancellationToken: cancellationToken);
        var trace = GetTrace();
        _currentTrace = trace?.StartActiveSpan("SqlServer: UseTransaction");
    }

    public async Task<ErrorOr<bool>> CommitAsync()
    {
        return await CommitAsync(CancellationToken.None);
    }
    public async Task<ErrorOr<bool>> CommitAsync(CancellationToken cancellationToken)
    {
        var trace = GetTrace();
        var span = trace?.StartActiveSpan("SqlServer: Commit");
        try
        {
            var entries = Context.ChangeTracker.Entries().ToEntityEntryItems();
            if (_currentTransaction is null)
                throw new Exception("please define Transaction first");

            var persistenceTransactionalEventEntity = Context.ChangeTracker.Entries<IPersistenceTransactionalEventEntity>();
            do
            {
                foreach (var entry in persistenceTransactionalEventEntity)
                {
                    if (entry.Entity.PersistenceTransactionalEvents.Count(e => e.ExecutionStatus == ExecutionStatusType.Initialized) == 0)
                        continue;
                    var domainResults = await domainEventDispatcher.DispatchAllAsync(entry.Entity.PersistenceTransactionalEvents.Where(e => e.ExecutionStatus == ExecutionStatusType.Initialized).ToList());
                    if (!domainResults.Success)
                    {
                        await RollbackAsync(cancellationToken);
                        return domainResults.Errors;
                    }
                }
                await SaveChangesAsync(cancellationToken);
                persistenceTransactionalEventEntity = Context.ChangeTracker.Entries<IPersistenceTransactionalEventEntity>();
            }
            while (!persistenceTransactionalEventEntity.All(ptee => ptee.Entity.PersistenceTransactionalEvents.All(e => e.ExecutionStatus != ExecutionStatusType.Initialized)));

            await _currentTransaction.CommitAsync(cancellationToken);

            foreach (var entry in entries)
            {
                if (entry.EntityEntry.Entity is IPersistenceEventEntity persistenceEventEntity)
                {
                    await domainEventDispatcher.DispatchAllAsync(persistenceEventEntity.PersistenceDomainEvents);
                    persistenceEventEntity.PersistenceDomainEvents.Clear();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
        finally
        {
            if (_currentTransaction != null)
                await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
            span?.End();
            _currentTrace?.End();
        }
    }

    public async Task RollbackAsync()
    {
        await RollbackAsync(CancellationToken.None);
    }
    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction is null)
            return;

        var trace = GetTrace();
        var span = trace?.StartActiveSpan("SqlServer: Rollback");

        await _currentTransaction.RollbackAsync(cancellationToken);
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
        span?.End();
        _currentTrace?.End();
    }


    public async Task<int> SaveChangesAsync()
    {
        return await SaveChangesAsync(CancellationToken.None);
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {

        Context.ChangeTracker.DetectChanges();
        var entries = Context.ChangeTracker.Entries();

        string? parentTraceId = null;
        foreach (var entry in entries)
        {
            if (entry.Metadata.GetRuntimeAnnotations().FirstOrDefault(item => item.Name == "traceId") is not IAnnotation annotation)
                continue;
            parentTraceId = annotation.Value?.ToString();
            break;
        }
        var trace = GetTrace();
        TelemetrySpan? span = trace?.StartActiveSpan("SqlServer: SaveChanges", parentTraceId);

        var entityEntryItems = entries.ToEntityEntryItems();
        var dateTimeEntityPropertySet = new DateTimeEntityPropertySet();
        foreach (var entity in entityEntryItems.Where(entry => entry.State == EntityState.Modified || entry.State == EntityState.Added))
        {
            await dateTimeEntityPropertySet.Set(new EntityPropertySetParameters(entity.EntityEntry));
        }
        var result = await Context.SaveChangesAsync(cancellationToken);
        if (CurrentTransaction is null && result > 0)
        {
            if (_sqlTracingConfig?.IncludeCommandText == true)
            {
            }

            foreach (var entry in entries)
            {
                if (entry.Entity is IPersistenceEventEntity persistenceDomainEventEntity)
                {
                    await domainEventDispatcher.DispatchAllAsync(persistenceDomainEventEntity.PersistenceDomainEvents);
                    persistenceDomainEventEntity.PersistenceDomainEvents.Clear();
                }
            }

            span?.End();
        }


        return result;
    }

}
