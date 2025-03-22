namespace Daric.Domain.Shared
{
    public interface IPersistenceTransactionalEventEntity
    {
        List<IDomainEvent> PersistenceTransactionalEvents { get; }
    }
}
