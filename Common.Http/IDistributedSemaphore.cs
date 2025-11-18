namespace Common.Interfaces;

public interface IDistributedSemaphore
{
    Task<IAsyncDisposable> WaitAsync(CancellationToken cancellationToken = default);
}