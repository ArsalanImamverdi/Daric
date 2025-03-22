namespace Daric.Domain.Shared
{
    public interface IDomainEventDispatcher
    {
        Task<ErrorOr<bool>> DispatchAsync(IDomainEvent domainEvent);
        Task<ErrorOr<bool>> DispatchAllAsync(List<IDomainEvent> domainEvents);
    }
}
