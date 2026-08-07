using DTP.Modules.Chatbot.Application.DTOs;

namespace DTP.Modules.Chatbot.Application.Abstractions
{
    public interface IChatbotConversationContextStore
    {
        Task<ChatbotIntentDto?> GetAsync(
            string sessionId,
            CancellationToken cancellationToken = default);

        Task SetAsync(
            string sessionId,
            ChatbotIntentDto intent,
            CancellationToken cancellationToken = default);
    }
}
