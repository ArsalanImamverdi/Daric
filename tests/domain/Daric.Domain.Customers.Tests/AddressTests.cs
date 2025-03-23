using Daric.Domain.Shared;

namespace Daric.Domain.Customers.Tests
{
    public class AddressTests
    {
        [Fact]
        public void CreateAddress_ValidData_ShouldSucceed()
        {
            var address = Address.Create("USA", "New York", "123 Main St", "10001");

            Assert.True(address);
            Assert.Equal("USA", address.Result!.Country);
            Assert.Equal("New York", address.Result.City);
            Assert.Equal("123 Main St", address.Result.Street);
            Assert.Equal("10001", address.Result.PostalCode);
        }

        [Fact]
        public void CreateAddress_InvalidCity_ShouldFail()
        {
            var result = Address.Create("USA", "", "123 Main St", "10001");

            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Code == ErrorCode.NullOrEmpty);
        }
    }
}
