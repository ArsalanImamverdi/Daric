using System.Linq.Expressions;

namespace Daric.Domain.Shared;

public interface IRepository { }
public interface IRepository<TModel> : IRepository
    where TModel : class
{
    Task Insert(TModel entity);
    Task Update(TModel entity);
    Task Delete(TModel entity);
    Task<IEnumerable<TModel>> FilterAsync(Expression<Func<TModel, bool>> predicate);
    Task BulkInsert(IEnumerable<TModel> entities);
    Task BulkUpdate(IEnumerable<TModel> entities);
    Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate);
    Task<TModel?> LastOrDefaultAsync(Expression<Func<TModel, bool>> predicate);
}
