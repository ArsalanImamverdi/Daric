namespace Daric.Database.Abstraction
{
    public interface IDbContextEventHandler
    {
        EventType Type { get; }

    }
}
