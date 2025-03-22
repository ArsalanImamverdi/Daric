namespace Daric.Locking.Abstraction;

public class DistributedSynchronizationHandle(IDisposable handle) : IDistributedSynchronizationHandle
{
    private readonly IDisposable _handle = handle ?? throw new ArgumentNullException(nameof(handle));

    public void Dispose()
    {
        _handle.Dispose();

        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_handle is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            _handle.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
