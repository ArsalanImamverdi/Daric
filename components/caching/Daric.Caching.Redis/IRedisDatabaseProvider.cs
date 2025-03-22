using StackExchange.Redis;

namespace Daric.Caching.Redis
{
    public interface IRedisDatabaseProvider
    {
        public Task<IDatabase> GetDatabaseAsync(CancellationToken cancellationToken);
        public Task<IDatabase> GetDatabaseAsync(string prefix, CancellationToken cancellationToken);
    }
}
