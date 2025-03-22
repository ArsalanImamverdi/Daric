using System.Data.Common;

namespace Daric.Domain.Shared;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task BeginTransactionAsync();
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task UseTransactionAsync(DbTransaction transaction);
    Task UseTransactionAsync(DbTransaction transaction, CancellationToken cancellationToken);
    Task<ErrorOr<bool>> CommitAsync();
    Task<ErrorOr<bool>> CommitAsync(CancellationToken cancellationToken);
    Task RollbackAsync();
    Task RollbackAsync(CancellationToken cancellationToken);
}
