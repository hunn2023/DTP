using DTP.Shared.Caching;
using StackExchange.Redis;
using System.Text.Json;

namespace DTP.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly StackExchange.Redis.IDatabase _database;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = redis.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(
            string key,
            CancellationToken cancellationToken = default)
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(
                key,
                json,
                (Expiration)expiration);
        }

        public async Task RemoveAsync(
            string key,
            CancellationToken cancellationToken = default)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task RemoveByPrefixAsync(
            string prefix,
            CancellationToken cancellationToken = default)
        {
            var endpoints = _redis.GetEndPoints();

            foreach (var endpoint in endpoints)
            {
                var server = _redis.GetServer(endpoint);

                var keys = server.Keys(pattern: $"{prefix}*");

                foreach (var key in keys)
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }
    }
}
