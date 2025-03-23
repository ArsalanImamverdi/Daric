using Daric.Domain.Customers.Internals;
using Daric.Domain.Shared;

namespace Daric.Domain.Customers;

public enum PhoneType
{
    Home,
    Work,
    Mobile
}
public sealed record Phone
{
    public string CountryCode { get; init; }
    public string PhoneNumber { get; init; }
    public PhoneType PhoneType { get; init; }
    public string FullPhoneNumber { get; init; }

    internal Phone()
    {
        CountryCode = string.Empty;
        PhoneNumber = string.Empty;
        PhoneType = PhoneType.Home;
        FullPhoneNumber = string.Empty;
    }
    internal Phone(string countryCode, string phoneNumber, PhoneType phoneType)
    {
        CountryCode = countryCode;
        PhoneNumber = phoneNumber;
        PhoneType = phoneType;
        FullPhoneNumber = string.Format("{0}{1}", countryCode, phoneNumber);
    }
    public static ErrorOr<Phone> Create(string phoneNumber)
    {
        return Create(string.Empty, phoneNumber);
    }
    public static ErrorOr<Phone> Create(string countryCode, string phoneNumber)
    {
        return Create(countryCode, phoneNumber, PhoneType.Mobile);
    }
    public static ErrorOr<Phone> Create(string countryCode, string phoneNumber, PhoneType phoneType)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            countryCode = Constants.IRCountryCode;
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return new NullOrEmptyError(nameof(PhoneNumber));
        return new ErrorOr<Phone>(new Phone(countryCode, phoneNumber, phoneType));
    }

}