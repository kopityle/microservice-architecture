using Common.Interfaces;
using StackExchange.Redis;

namespace TrackingService.Dal.DistributedLocks;

public class RedisDistributedSemaphore : IDistributedSemaphore
{
    private readonly IDatabase _database;
    private readonly string _key;
    private readonly int _maxCount;
    // Время жизни ключа, чтобы если сервер упадет, блокировка сама снялась через минуту
    private readonly int _expirySeconds = 60;

    public RedisDistributedSemaphore(string resourceName, int maxCount, IConnectionMultiplexer connection)
    {
        _database = connection.GetDatabase();
        _key = $"semaphore:{resourceName}";
        _maxCount = maxCount;
    }

    public async Task<IAsyncDisposable> WaitAsync(CancellationToken cancellationToken = default)
    {
        // Lua-скрипт для захвата семафора
        // KEYS[1] - имя ключа
        // ARGV[1] - лимит (maxCount)
        // ARGV[2] - время жизни в секундах
        const string script = @"
            local count = redis.call('INCR', KEYS[1])
            if count <= tonumber(ARGV[1]) then
                redis.call('EXPIRE', KEYS[1], ARGV[2])
                return 1
            else
                redis.call('DECR', KEYS[1])
                return 0
            end";

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                // Выполняем скрипт
                var result = (int)await _database.ScriptEvaluateAsync(
                    script,
                    new RedisKey[] { _key }, // KEYS
                    new RedisValue[] { _maxCount, _expirySeconds } // ARGVS
                );

                if (result == 1)
                {
                    // Успех!
                    return new RedisSemaphoreReleaser(_database, _key);
                }
            }
            catch (Exception ex)
            {
                // Логируем или игнорируем ошибки Redis, пробуем снова
                Console.WriteLine($"Redis Error: {ex.Message}");
            }

            // Ждем 500мс перед следующей попыткой
            await Task.Delay(500, cancellationToken);
        }
    }

    private class RedisSemaphoreReleaser : IAsyncDisposable
    {
        private readonly IDatabase _db;
        private readonly string _key;

        public RedisSemaphoreReleaser(IDatabase db, string key)
        {
            _db = db;
            _key = key;
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _db.StringDecrementAsync(_key);
            }
            catch
            {
                // Ошибки при освобождении не должны ломать приложение
            }
        }
    }
}