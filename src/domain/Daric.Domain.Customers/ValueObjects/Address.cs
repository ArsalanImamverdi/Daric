using Daric.Domain.Customers.Internals;
using Daric.Domain.Shared;

namespace Daric.Domain.Customers;

public sealed record Address
{
    public string Country { get; init; }
    public string City { get; init; }
    public string Street { get; init; }
    public string PostalCode { get; init; }

    internal Address()
    {
        Country = string.Empty;
        City = string.Empty;
        Street = string.Empty;
        PostalCode = string.Empty;
    }
    internal Address(string country, string city, string street, string postalCode)
    {
        Country = country;
        City = city;
        Street = street;
        PostalCode = postalCode;
    }

    public static ErrorOr<Address> Create(string city, string street, string postalCode)
    {
        return Create(string.Empty, city, street, postalCode);
    }

    public static ErrorOr<Address> Create(string country, string city, string street, string postalCode)
    {
        var errors = new List<Error>();
        if (string.IsNullOrWhiteSpace(country))
            country = Constants.IR;
        if (string.IsNullOrWhiteSpace(city))
            errors.Add(new NullOrEmptyError(nameof(City)));
        if (string.IsNullOrWhiteSpace(street))
            errors.Add(new NullOrEmptyError(nameof(Street)));
        if (string.IsNullOrWhiteSpace(postalCode))
            errors.Add(new NullOrEmptyError(nameof(PostalCode)));

        if (errors.Count > 0)
            return errors;

        return new Address(country, city, street, postalCode);
    }
}
