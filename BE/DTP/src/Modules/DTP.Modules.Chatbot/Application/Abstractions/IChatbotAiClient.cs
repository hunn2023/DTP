using DTP.Modules.Chatbot.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.Abstractions
{
    public interface IChatbotAiClient
    {
        Task<ChatbotIntentDto> ExtractIntentAsync(
            string message,
            CancellationToken cancellationToken = default);

        Task<string> GenerateAnswerAsync(
            string userMessage,
            ChatbotIntentDto intent,
            IReadOnlyList<ChatbotProductSuggestionDto> suggestions,
            CancellationToken cancellationToken = default);
    }
}
