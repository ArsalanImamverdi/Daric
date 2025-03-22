namespace Daric.Database.Abstraction
{
    public interface IDbContextEventManager
    {
        Task AddHandler(string key, IDbContextEventHandler handler);
        Task Manage(IDbContextEventManagerArg arg);
    }
}
