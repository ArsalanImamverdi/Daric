namespace Daric.Domain.Shared
{
    public interface IDomainEventEntity
    {
        List<IDomainEvent> DomainEvents { get; }
    }
}
