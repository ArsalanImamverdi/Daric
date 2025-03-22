namespace Daric.Domain.Shared
{
    public interface IPersistenceEventEntity
    {
        List<IDomainEvent> PersistenceDomainEvents { get; }
    }
}
