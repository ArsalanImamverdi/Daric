using Daric.Database.SqlServer;
using Daric.Domain.Shared;

namespace Daric.Infrastructure.SqlServer
{
    internal class DaricUnitOfWork(DaricDbContext context, IDomainEventDispatcher domainEventDispatcher, IServiceProvider serviceProvider)
        : SqlServerUnitOfWork<DaricDbContext>(context, domainEventDispatcher, serviceProvider)
    {
    }
}
