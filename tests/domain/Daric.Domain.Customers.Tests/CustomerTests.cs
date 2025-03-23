using Daric.Domain.Customers.DomainErrors;
using Daric.Domain.Shared;

namespace Daric.Domain.Customers.Tests
{
    public class CustomerTests
    {
        [Fact]
        public void CreateCustomer_ValidData_ShouldSucceed()
        {
            var phone = Phone.Create("+1", "1234567890", PhoneType.Mobile);
            Assert.True(phone);

            var address = Address.Create("USA", "New York", "123 Main St", "10001");
            Assert.True(address);

            var customer = Customer.Create("John", "Doe", new DateTime(1990, 1, 1), [phone!], [address!]).Result;

            Assert.NotNull(customer);
            Assert.Equal("John", customer.FirstName);
            Assert.Equal("Doe", customer.LastName);
            Assert.Equal(new DateTime(1990, 1, 1), customer.DateOfBirth);
            Assert.Single(customer.Phones);
            Assert.Single(customer.Addresses);
        }

        [Fact]
        public void CreateCustomer_InvalidFirstName_ShouldFail()
        {
            var result = Customer.Create("", "Doe", new DateTime(1990, 1, 1), [], []);

            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Code == ErrorCode.NullOrEmpty);
        }

        [Fact]
        public void CreateCustomer_InvalidDateOfBirth_ShouldFail()
        {
            var result = Customer.Create("John", "Doe", DateTime.MinValue, [], []);

            Assert.False(result);
            Assert.Contains(result.Errors, e => e is AgeError);
        }
    }
}