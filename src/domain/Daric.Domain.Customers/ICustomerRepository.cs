using Daric.Domain.Shared;

namespace Daric.Domain.Customers
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        public Task<Customer[]> GetTransferCustomersAsync(Guid customerId, Guid receiverCustomerId, CancellationToken cancellationToken);
    }
}
