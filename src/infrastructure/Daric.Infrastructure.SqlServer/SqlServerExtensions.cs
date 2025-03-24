using Daric.Database.Abstraction;
using Daric.Database.SqlServer.Extensions;
using Daric.Domain.Accounts;
using Daric.Domain.Customers;
using Daric.Domain.Transactions;
using Daric.Domain.Transactions.DomainServices;
using Daric.Infrastructure.SqlServer.DomainServices;
using Daric.Infrastructure.SqlServer.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace Daric.Infrastructure.SqlServer
{
    public static class SqlServerExtensions
    {
        public static IServiceCollection AddDaricDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDatabase(db => db.AddSqlDatabase(sql => sql.AddContext<DaricDbContext>(opt => opt.AddRepository<ICustomerRepository, CustomerRepository>()
                                                                                                                  .AddRepository<ITransactionRepository, TransactionRepository>()
                                                                                                                  .AddRepository<IAccountRepository, AccountRepository>()
                                                                                                                  .WithUnitOfWork<DaricUnitOfWork>())));
            
            serviceCollection.AddScoped<IAccountNumberGenerator, AccountNumberGenerator>();
            serviceCollection.AddScoped<ITrackingCodeGenerator, TrackingCodeGenerator>();

            return serviceCollection;
        }
    }
}
