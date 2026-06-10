using DTP.Modules.Ordering.Application.Abstractions.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderRateLimitService : IOrderRateLimitService
    {
        private readonly IDatabase _redis;

        private const int MaxCreateOrderByUser = 5;
        private const int MaxCreateOrderByIp = 20;

        private static readonly TimeSpan CreateOrderTtl = TimeSpan.FromMinutes(10);

        public OrderRateLimitService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<bool> IsCreateOrderBlockedAsync(
            Guid userId,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            var userCount = await GetIntAsync(OrderRateLimitKeys.CreateOrderUser(userId));
            var ipCount = await GetIntAsync(OrderRateLimitKeys.CreateOrderIp(ipAddress));

            return userCount >= MaxCreateOrderByUser || ipCount >= MaxCreateOrderByIp;
        }

        public async Task RegisterCreateOrderAttemptAsync(
            Guid userId,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            ipAddress = NormalizeIp(ipAddress);

            var userKey = OrderRateLimitKeys.CreateOrderUser(userId);
            var ipKey = OrderRateLimitKeys.CreateOrderIp(ipAddress);

            var userCount = await _redis.StringIncrementAsync(userKey);
            var ipCount = await _redis.StringIncrementAsync(ipKey);

            if (userCount == 1)
                await _redis.KeyExpireAsync(userKey, CreateOrderTtl);

            if (ipCount == 1)
                await _redis.KeyExpireAsync(ipKey, CreateOrderTtl);
        }

        private async Task<int> GetIntAsync(string key)
        {
            var value = await _redis.StringGetAsync(key);

            if (!value.HasValue)
                return 0;

            return int.TryParse(value.ToString(), out var result)
                ? result
                : 0;
        }

        private static string NormalizeIp(string ip)
        {
            return string.IsNullOrWhiteSpace(ip) ? "unknown" : ip.Trim();
        }
    }

    public static class OrderRateLimitKeys
    {
        public static string CreateOrderUser(Guid userId)
            => $"ordering:create:user:{userId}";

        public static string CreateOrderIp(string ip)
            => $"ordering:create:ip:{ip}";
    }
}
