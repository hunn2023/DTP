using DTP.Modules.Auth.Application.Abstractions.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IDatabase _database;

        public RateLimitService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> IsAllowedAsync(
            string key,
            int maxRequest,
            TimeSpan window)
        {
            var count = await _database.StringIncrementAsync(key);

            if (count == 1)
            {
                await _database.KeyExpireAsync(key, window);
            }

            return count <= maxRequest;
        }
    }
}
