using Common.Interfaces;
using Medallion.Threading.Redis; // Из библиотеки DistributedLock
using StackExchange.Redis;

namespace TrackingService.Dal.DistributedLocks;

public class RedisDistributedSemaphore : IDistributedSemaphore
{
    // Это настоящая реализация из библиотеки, которую мы "прячем"
    private readonly Medallion.Threading.Redis.RedisDistributedSemaphore _underlyingSemaphore;

    // Наш конструктор просто создает экземпляр семафора из библиотеки
    public RedisDistributedSemaphore(string resourceName, int maxCount, IConnectionMultiplexer connection)
    {
        _underlyingSemaphore = new Medallion.Threading.Redis.RedisDistributedSemaphore(resourceName, maxCount, connection.GetDatabase());
    }

    // Наш метод просто "пробрасывает" вызов в настоящую реализацию
    public async Task<IAsyncDisposable> WaitAsync(CancellationToken cancellationToken = default)
    {
        return await _underlyingSemaphore.AcquireAsync(cancellationToken: cancellationToken);
    }
}