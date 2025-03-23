using Daric.Domain.Customers.DomainErrors;
using Daric.Domain.Customers.Internals;
using Daric.Domain.Shared;

namespace Daric.Domain.Customers;

public sealed class Customer : IAggregateRoot
{
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public IReadOnlyList<Phone> Phones { get; private set; }
    public IReadOnlyList<Address> Addresses { get; private set; }

    private Customer()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        DateOfBirth = DateTime.MinValue;
        Phones = [];
        Addresses = [];
    }
    private Customer(string firstName, string lastName, DateTime dateOfBirth, IReadOnlyList<Phone> phones, IReadOnlyList<Address> addresses)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Phones = phones;
        Addresses = addresses;
    }

    public static ErrorOr<Customer> Create(string firstName, string lastName, DateTime dateOfBirth, Phone[] phones, Address[] addresses)
    {
        List<Error> errors = [];

        if (string.IsNullOrWhiteSpace(firstName))
            errors.Add(new NullOrEmptyError(nameof(FirstName)));

        if (string.IsNullOrWhiteSpace(lastName))
            errors.Add(new NullOrEmptyError(nameof(LastName)));

        int age = CalculateAge(dateOfBirth);
        if (age < Constants.MIN_AGE || age > Constants.MAX_AGE)
            errors.Add(new AgeError());

        if (addresses is null || addresses.Length == 0)
            errors.Add(new NullOrEmptyError(nameof(Addresses)));

        if (phones is null || phones.Length == 0)
            errors.Add(new NullOrEmptyError(nameof(Phones)));

        if (errors.Count > 0)
            return errors;

        return new Customer(firstName, lastName, dateOfBirth, phones!, addresses!);
    }

    private static int CalculateAge(DateTime birthDate)
    {
        DateTime today = DateTime.Today;
        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age;
    }
}