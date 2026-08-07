using System.Text.Json;
using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using StackExchange.Redis;

namespace DTP.Modules.Chatbot.Application.Services
{
    public sealed class RedisChatbotConversationContextStore
        : IChatbotConversationContextStore
    {
        private static readonly TimeSpan ContextLifetime = TimeSpan.FromMinutes(30);
        private readonly IConnectionMultiplexer _redis;

        public RedisChatbotConversationContextStore(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<ChatbotIntentDto?> GetAsync(
            string sessionId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var value = await _redis.GetDatabase().StringGetAsync(BuildKey(sessionId));

            return value.IsNullOrEmpty
                ? null
                : JsonSerializer.Deserialize<ChatbotIntentDto>(value.ToString());
        }

        public async Task SetAsync(
            string sessionId,
            ChatbotIntentDto intent,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _redis.GetDatabase().StringSetAsync(
                BuildKey(sessionId),
                JsonSerializer.Serialize(intent),
                ContextLifetime);
        }

        private static string BuildKey(string sessionId)
        {
            return $"chatbot:context:{sessionId.Trim()}";
        }
    }
}
