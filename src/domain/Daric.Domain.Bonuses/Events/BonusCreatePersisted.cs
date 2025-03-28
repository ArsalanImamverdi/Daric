using Daric.Domain.Shared;

namespace Daric.Domain.Bonuses.Events
{
    public record BonusCreatePersisted(Guid AccountId, string Description, decimal Amount) : DomainEvent
    {
    }
}
