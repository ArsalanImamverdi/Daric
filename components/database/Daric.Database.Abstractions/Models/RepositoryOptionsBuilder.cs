using Microsoft.Extensions.DependencyInjection;
using Daric.Domain.Shared;

namespace Daric.Database.Abstraction
{
    public abstract class RepositoryOptionsBuilder<TContext>
        where TContext : IDbContext
    {
        public IServiceCollection ServiceCollection { get; }

        protected RepositoryOptionsBuilder(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
        }

        public abstract RepositoryOptionsBuilder<TContext> AddRepository<TRepository, TImp>()
            where TRepository : IRepository
            where TImp : class, TRepository;
    }
}
