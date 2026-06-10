using DTP.Modules.Delivery.Application.Abstractions.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Infrastructure.Services
{
    public class DeliveryRateLimitService : IDeliveryRateLimitService
    {
        private readonly IDatabase _redis;

        private const int MaxProcessByDelivery = 3;
        private const int MaxProcessByOrder = 3;
        private const int MaxProcessByIp = 20;

        private const int MaxRetryByDelivery = 3;
        private const int MaxRetryByOrder = 3;
        private const int MaxRetryByIp = 10;

        private static readonly TimeSpan ProcessTtl = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan RetryTtl = TimeSpan.FromMinutes(30);

        public DeliveryRateLimitService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<bool> IsProcessBlockedAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);

            var deliveryCount = await GetIntAsync(DeliveryRateLimitKeys.ProcessDelivery(deliveryId));
            var orderCount = await GetIntAsync(DeliveryRateLimitKeys.ProcessOrder(orderId));
            var ipCount = await GetIntAsync(DeliveryRateLimitKeys.ProcessIp(ip));

            return deliveryCount >= MaxProcessByDelivery ||
                   orderCount >= MaxProcessByOrder ||
                   ipCount >= MaxProcessByIp;
        }

        public async Task RegisterProcessAttemptAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);

            await IncrementWithTtlAsync(DeliveryRateLimitKeys.ProcessDelivery(deliveryId), ProcessTtl);
            await IncrementWithTtlAsync(DeliveryRateLimitKeys.ProcessOrder(orderId), ProcessTtl);
            await IncrementWithTtlAsync(DeliveryRateLimitKeys.ProcessIp(ip), ProcessTtl);
        }

        public async Task<bool> IsRetryBlockedAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);

            var deliveryCount = await GetIntAsync(DeliveryRateLimitKeys.RetryDelivery(deliveryId));
            var orderCount = await GetIntAsync(DeliveryRateLimitKeys.RetryOrder(orderId));
            var ipCount = await GetIntAsync(DeliveryRateLimitKeys.RetryIp(ip));

            return deliveryCount >= MaxRetryByDelivery ||
                   orderCount >= MaxRetryByOrder ||
                   ipCount >= MaxRetryByIp;
        }

        public async Task RegisterRetryAttemptAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);

            await IncrementWithTtlAsync(DeliveryRateLimitKeys.RetryDelivery(deliveryId), RetryTtl);
            await IncrementWithTtlAsync(DeliveryRateLimitKeys.RetryOrder(orderId), RetryTtl);
            await IncrementWithTtlAsync(DeliveryRateLimitKeys.RetryIp(ip), RetryTtl);
        }

        private async Task IncrementWithTtlAsync(string key, TimeSpan ttl)
        {
            var count = await _redis.StringIncrementAsync(key);

            if (count == 1)
                await _redis.KeyExpireAsync(key, ttl);
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

        private static string NormalizeIp(string? ipAddress)
        {
            return string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();
        }
    }

    public static class DeliveryRateLimitKeys
    {
        public static string ProcessDelivery(Guid deliveryId)
            => $"delivery:process:delivery:{deliveryId}";

        public static string ProcessOrder(Guid orderId)
            => $"delivery:process:order:{orderId}";

        public static string ProcessIp(string ip)
            => $"delivery:process:ip:{ip}";

        public static string RetryDelivery(Guid deliveryId)
            => $"delivery:retry:delivery:{deliveryId}";

        public static string RetryOrder(Guid orderId)
            => $"delivery:retry:order:{orderId}";

        public static string RetryIp(string ip)
            => $"delivery:retry:ip:{ip}";
    }
}
