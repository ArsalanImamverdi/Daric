namespace Daric.Database.Abstraction
{
    public interface IDbContextEventHandler<TEvent> : IDbContextEventHandler
        where TEvent : IDbContextEventArgs
    {
        Task Handle(TEvent @event);
    }
}
