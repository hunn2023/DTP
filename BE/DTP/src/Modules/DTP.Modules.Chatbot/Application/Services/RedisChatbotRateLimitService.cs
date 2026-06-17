using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.Services
{
    public sealed class RedisChatbotRateLimitService : IChatbotRateLimitService
    {
        private readonly IConnectionMultiplexer _redis;

        private const int MaxMessageLength = 1000;

        private const int MaxIpPerMinute = 20;
        private const int MaxSessionPerMinute = 10;
        private const int MaxUserPerMinute = 30;

        private const int MaxSameMessagePerMinute = 3;

        private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(5);

        public RedisChatbotRateLimitService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<ChatbotRateLimitResult> CheckAsync(
            ChatbotRateLimitContext context,
            CancellationToken cancellationToken = default)
        {
            var message = context.Message?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(message))
            {
                return ChatbotRateLimitResult.Failed(
                    reason: "EMPTY_MESSAGE",
                    message: "Bạn vui lòng nhập nội dung cần tư vấn.");
            }

            if (message.Length > MaxMessageLength)
            {
                return ChatbotRateLimitResult.Failed(
                    reason: "MESSAGE_TOO_LONG",
                    message: $"Nội dung quá dài. Bạn vui lòng nhập tối đa {MaxMessageLength} ký tự.");
            }

            var db = _redis.GetDatabase();

            var ip = NormalizeKeyPart(context.IpAddress);
            var sessionId = NormalizeKeyPart(context.SessionId);
            var userId = NormalizeKeyPart(context.UserId);

            var ipBlockKey = $"chatbot:limit:block:ip:{ip}";
            var sessionBlockKey = $"chatbot:limit:block:session:{sessionId}";
            var userBlockKey = string.IsNullOrWhiteSpace(userId)
                ? null
                : $"chatbot:limit:block:user:{userId}";

            if (await db.KeyExistsAsync(ipBlockKey))
            {
                return ChatbotRateLimitResult.Failed(
                    reason: "IP_BLOCKED",
                    message: "Bạn đang gửi yêu cầu quá nhanh. Vui lòng thử lại sau vài phút.",
                    retryAfterSeconds: 300);
            }

            if (!string.IsNullOrWhiteSpace(sessionId)
                && await db.KeyExistsAsync(sessionBlockKey))
            {
                return ChatbotRateLimitResult.Failed(
                    reason: "SESSION_BLOCKED",
                    message: "Phiên chat đang gửi yêu cầu quá nhanh. Vui lòng thử lại sau vài phút.",
                    retryAfterSeconds: 300);
            }

            if (!string.IsNullOrWhiteSpace(userBlockKey)
                && await db.KeyExistsAsync(userBlockKey))
            {
                return ChatbotRateLimitResult.Failed(
                    reason: "USER_BLOCKED",
                    message: "Tài khoản của bạn đang gửi yêu cầu quá nhanh. Vui lòng thử lại sau vài phút.",
                    retryAfterSeconds: 300);
            }

            var ipResult = await IncreaseAndCheckAsync(
                db,
                key: $"chatbot:limit:ip:{ip}",
                max: MaxIpPerMinute,
                window: Window);

            if (!ipResult.Allowed)
            {
                await db.StringSetAsync(ipBlockKey, "1", BlockDuration);

                return ChatbotRateLimitResult.Failed(
                    reason: "IP_RATE_LIMIT_EXCEEDED",
                    message: "Bạn gửi tin nhắn quá nhiều lần. Vui lòng thử lại sau vài phút.",
                    retryAfterSeconds: 300);
            }

            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                var sessionResult = await IncreaseAndCheckAsync(
                    db,
                    key: $"chatbot:limit:session:{sessionId}",
                    max: MaxSessionPerMinute,
                    window: Window);

                if (!sessionResult.Allowed)
                {
                    await db.StringSetAsync(sessionBlockKey, "1", BlockDuration);

                    return ChatbotRateLimitResult.Failed(
                        reason: "SESSION_RATE_LIMIT_EXCEEDED",
                        message: "Bạn gửi tin nhắn quá nhanh trong phiên chat này. Vui lòng thử lại sau vài phút.",
                        retryAfterSeconds: 300);
                }
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userResult = await IncreaseAndCheckAsync(
                    db,
                    key: $"chatbot:limit:user:{userId}",
                    max: MaxUserPerMinute,
                    window: Window);

                if (!userResult.Allowed)
                {
                    await db.StringSetAsync(userBlockKey!, "1", BlockDuration);

                    return ChatbotRateLimitResult.Failed(
                        reason: "USER_RATE_LIMIT_EXCEEDED",
                        message: "Tài khoản của bạn gửi tin nhắn quá nhiều lần. Vui lòng thử lại sau vài phút.",
                        retryAfterSeconds: 300);
                }
            }

            var messageHash = HashMessage(message);
            var actorKey = !string.IsNullOrWhiteSpace(userId)
                ? $"user:{userId}"
                : $"session:{sessionId}";

            var repeatResult = await IncreaseAndCheckAsync(
                db,
                key: $"chatbot:limit:repeat:{actorKey}:{messageHash}",
                max: MaxSameMessagePerMinute,
                window: Window);

            if (!repeatResult.Allowed)
            {
                if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(userBlockKey))
                {
                    await db.StringSetAsync(userBlockKey, "1", BlockDuration);
                }
                else if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    await db.StringSetAsync(sessionBlockKey, "1", BlockDuration);
                }

                return ChatbotRateLimitResult.Failed(
                    reason: "SAME_MESSAGE_REPEATED",
                    message: "Bạn đang gửi lặp lại cùng một nội dung quá nhiều lần. Vui lòng thay đổi nội dung hoặc thử lại sau.",
                    retryAfterSeconds: 300);
            }

            return ChatbotRateLimitResult.Success();
        }

        private static async Task<CounterResult> IncreaseAndCheckAsync(
            IDatabase db,
            string key,
            int max,
            TimeSpan window)
        {
            var count = await db.StringIncrementAsync(key);

            if (count == 1)
            {
                await db.KeyExpireAsync(key, window);
            }

            return new CounterResult
            {
                Allowed = count <= max,
                Count = count
            };
        }

        private static string NormalizeKeyPart(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Guid.NewGuid().ToString();

            return value
                .Trim()
                .ToLowerInvariant()
                .Replace(":", "_")
                .Replace("/", "_")
                .Replace("\\", "_")
                .Replace(" ", "_");
        }

        private static string HashMessage(string message)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(message.Trim().ToLowerInvariant()));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private sealed class CounterResult
        {
            public bool Allowed { get; set; }

            public long Count { get; set; }
        }
    }
}
