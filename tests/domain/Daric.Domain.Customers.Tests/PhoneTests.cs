using Daric.Domain.Shared;

namespace Daric.Domain.Customers.Tests
{
    public class PhoneTests
    {
        [Fact]
        public void CreatePhone_ValidData_ShouldSucceed()
        {
            var phone = Phone.Create("+1", "1234567890", PhoneType.Mobile);

            Assert.True(phone);
            Assert.Equal("+1", phone.Result!.CountryCode);
            Assert.Equal("1234567890", phone.Result.PhoneNumber);
            Assert.Equal(PhoneType.Mobile, phone.Result.PhoneType);
        }

        [Fact]
        public void CreatePhone_InvalidPhoneNumber_ShouldFail()
        {
            var result = Phone.Create("+1", "", PhoneType.Mobile);

            Assert.False(result);
            Assert.Contains(result.Errors, e => e.Code == ErrorCode.NullOrEmpty);
        }
    }
}
