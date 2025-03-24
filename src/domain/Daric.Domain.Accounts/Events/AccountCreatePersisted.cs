using Daric.Domain.Shared;

namespace Daric.Domain.Accounts.DomainEvents
{
    public record AccountCreatePersistResult(long TrackingCode, DateTime CreatedAt) : IDomainEventResult;
    public record AccountCreatePersisted(string AccountNumber, Guid CustomerId) : DomainEvent<AccountCreatePersistResult>;
}
