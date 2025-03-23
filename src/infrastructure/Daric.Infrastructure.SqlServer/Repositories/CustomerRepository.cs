using Daric.Database.SqlServer;
using Daric.Domain.Customers;

using Microsoft.EntityFrameworkCore;

namespace Daric.Infrastructure.SqlServer.Repositories
{
    internal class CustomerRepository(DbSet<Customer> entity, IServiceProvider serviceProvider) : SqlServerRepository<Customer>(entity, serviceProvider), ICustomerRepository
    {
        public Task<Customer[]> GetTransferCustomersAsync(Guid customerId, Guid receiverCustomerId, CancellationToken cancellationToken)
        {
            return Entity.AsNoTracking().Where(customer => customer.Id == customerId || customer.Id == receiverCustomerId).ToArrayAsync(cancellationToken);
        }
    }
}
