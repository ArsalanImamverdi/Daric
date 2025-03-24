using Daric.Domain.Shared;

namespace Daric.Domain.Accounts.DomainEvents
{
    public record AccountBalanceUpdated(Account Account) : DomainEvent
    {
    }
}
