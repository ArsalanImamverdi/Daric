namespace Daric.Caching.Abstractions;

public interface IDistributedCacheDatabase
{
    Task SecureSetAsync(string key, decimal value, CancellationToken cancellationToken);
    Task<decimal> SecureGetAsync(string key, CancellationToken cancellationToken);
}
