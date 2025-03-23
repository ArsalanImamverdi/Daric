using Daric.Database.Abstraction;
using Daric.Database.SqlServer.Extensions;
using Daric.Domain.Customers;
using Daric.Infrastructure.SqlServer.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace Daric.Infrastructure.SqlServer
{
    public static class SqlServerExtensions
    {
        public static IServiceCollection AddDaricDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDatabase(db => db.AddSqlDatabase(sql => sql.AddContext<DaricDbContext>(opt => opt.AddRepository<ICustomerRepository, CustomerRepository>()
                                                                                                                  .WithUnitOfWork<DaricUnitOfWork>())));
            return serviceCollection;
        }
    }
}
