namespace Daric.Domain.Shared
{
    public interface IDomainEventHandler
    {
        Task<ErrorOr<bool>> HandleAsync(IDomainEvent @event, CancellationToken cancellationToken);
    }
    public interface IDomainEventHandler<TEvent> : IDomainEventHandler where TEvent : IDomainEvent
    {
        Task<ErrorOr<bool>> HandleAsync(TEvent @event, CancellationToken cancellationToken);
    }

    public abstract class DomainEventHandler<TEvent> : IDomainEventHandler<TEvent> where TEvent : IDomainEvent
    {
        public abstract Task<ErrorOr<bool>> HandleAsync(TEvent @event, CancellationToken cancellationToken);

        Task<ErrorOr<bool>> IDomainEventHandler.HandleAsync(IDomainEvent @event, CancellationToken cancellationToken)
        {
            if (@event is TEvent theEvent)
                return (this as IDomainEventHandler<TEvent>).HandleAsync(theEvent, cancellationToken);

            return Task.FromResult(new ErrorOr<bool>([new InvalidOperationError(string.Format("Can not execute {0} in {1}", @event.GetType().Name, GetType().Name))]));
        }
    }
}
