using Daric.Domain.Customers;
using Daric.Domain.Shared;

using Microsoft.Extensions.DependencyInjection;

namespace Dirac.Infrastructure.Tests.Customer
{
    [Collection("CustomerTests")]
    public class CustomerRepositoryTests
    {
        private readonly CustomerTestFixture _customerTestFixture;
        public CustomerRepositoryTests(CustomerTestFixture customerTestFixture)
        {
            _customerTestFixture = customerTestFixture;
        }
        [Fact]
        public async Task AddCustomer_ValidData_ShouldSucceed()
        {
            var customerRepository = _customerTestFixture.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var unitOfWork = _customerTestFixture.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var phone = Phone.Create("915");
            Assert.True(phone);

            var address = Address.Create("London", "Str", "96587");
            Assert.True(address);

            var customer = Daric.Domain.Customers.Customer.Create("Json", "Da", new DateTime(1990, 1, 1), [phone!], [address!]);
            Assert.True(customer);
            await customerRepository.Insert(customer!);
            var result = await unitOfWork.SaveChangesAsync();
            Assert.True(result >= 1);

            Assert.True(customer.Result!.Id != Guid.Empty);
        }
    }
}