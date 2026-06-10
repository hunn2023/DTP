using DTP.Modules.Payment.Application.Abstractions.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class PaymentRateLimitService : IPaymentRateLimitService
    {
        private readonly IDatabase _redis;

        private const int MaxCreateQrByOrder = 3;
        private const int MaxCreateQrByCustomer = 10;
        private const int MaxCreateQrByIp = 20;

        private static readonly TimeSpan CreateQrTtl = TimeSpan.FromMinutes(10);

        private const int MaxCallbackByTransaction = 10;
        private const int MaxCallbackByIp = 60;

        private static readonly TimeSpan CallbackTtl = TimeSpan.FromMinutes(10);

        public PaymentRateLimitService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<bool> IsCreateQrBlockedAsync(
            Guid orderId,
            Guid? customerId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);

            var orderCount = await GetIntAsync(PaymentRateLimitKeys.CreateQrOrder(orderId));
            var ipCount = await GetIntAsync(PaymentRateLimitKeys.CreateQrIp(ip));

            if (orderCount >= MaxCreateQrByOrder)
                return true;

            if (ipCount >= MaxCreateQrByIp)
                return true;

            if (customerId.HasValue && customerId.Value != Guid.Empty)
            {
                var customerCount = await GetIntAsync(
                    PaymentRateLimitKeys.CreateQrCustomer(customerId.Value));

                if (customerCount >= MaxCreateQrByCustomer)
                    return true;
            }

            return false;
        }

        public async Task RegisterCreateQrAttemptAsync(
            Guid orderId,
            Guid? customerId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);

            await IncrementWithTtlAsync(
                PaymentRateLimitKeys.CreateQrOrder(orderId),
                CreateQrTtl);

            await IncrementWithTtlAsync(
                PaymentRateLimitKeys.CreateQrIp(ip),
                CreateQrTtl);

            if (customerId.HasValue && customerId.Value != Guid.Empty)
            {
                await IncrementWithTtlAsync(
                    PaymentRateLimitKeys.CreateQrCustomer(customerId.Value),
                    CreateQrTtl);
            }
        }

        public async Task<bool> IsCallbackBlockedAsync(
            string? transactionCode,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);
            var tx = NormalizeTransaction(transactionCode);

            var ipCount = await GetIntAsync(PaymentRateLimitKeys.CallbackIp(ip));

            if (ipCount >= MaxCallbackByIp)
                return true;

            if (tx != "unknown")
            {
                var txCount = await GetIntAsync(PaymentRateLimitKeys.CallbackTransaction(tx));

                if (txCount >= MaxCallbackByTransaction)
                    return true;
            }

            return false;
        }

        public async Task RegisterCallbackAttemptAsync(
            string? transactionCode,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            var ip = NormalizeIp(ipAddress);
            var tx = NormalizeTransaction(transactionCode);

            await IncrementWithTtlAsync(
                PaymentRateLimitKeys.CallbackIp(ip),
                CallbackTtl);

            if (tx != "unknown")
            {
                await IncrementWithTtlAsync(
                    PaymentRateLimitKeys.CallbackTransaction(tx),
                    CallbackTtl);
            }
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

        private static string NormalizeIp(string? ip)
        {
            return string.IsNullOrWhiteSpace(ip)
                ? "unknown"
                : ip.Trim();
        }

        private static string NormalizeTransaction(string? transactionCode)
        {
            return string.IsNullOrWhiteSpace(transactionCode)
                ? "unknown"
                : transactionCode.Trim().ToLowerInvariant();
        }
    }

    public static class PaymentRateLimitKeys
    {
        public static string CreateQrOrder(Guid orderId)
            => $"payment:createqr:order:{orderId}";

        public static string CreateQrCustomer(Guid customerId)
            => $"payment:createqr:customer:{customerId}";

        public static string CreateQrIp(string ip)
            => $"payment:createqr:ip:{ip}";

        public static string CallbackTransaction(string transactionCode)
            => $"payment:callback:tx:{transactionCode}";

        public static string CallbackIp(string ip)
            => $"payment:callback:ip:{ip}";
    }
}
