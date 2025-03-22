using Microsoft.Extensions.Options;

using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;

namespace Daric.Caching.Redis.Internals
{
    internal sealed class RedisDatabaseProvider(IOptionsMonitor<ConnectionMultiplexerConnect> connectionMultiplexerConnect) : IRedisDatabaseProvider
    {
        public Task<IDatabase> GetDatabaseAsync(CancellationToken cancellationToken)
        {
            return connectionMultiplexerConnect.CurrentValue.ConnectTask!.ContinueWith(res => res.Result.GetDatabase(), cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);
        }

        public Task<IDatabase> GetDatabaseAsync(string prefix, CancellationToken cancellationToken)
        {
            return connectionMultiplexerConnect.CurrentValue.ConnectTask!.ContinueWith(res => res.Result.GetDatabase().WithKeyPrefix(prefix), cancellationToken, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);

        }
    }
}
