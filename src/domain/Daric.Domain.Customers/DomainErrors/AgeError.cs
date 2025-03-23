using Daric.Domain.Customers.Internals;
using Daric.Domain.Shared;

namespace Daric.Domain.Customers.DomainErrors
{
    public record AgeError() : InvalidDataError(string.Format("Age must be between {0},{1}", Constants.MIN_AGE, Constants.MAX_AGE))
    {
    }
}
