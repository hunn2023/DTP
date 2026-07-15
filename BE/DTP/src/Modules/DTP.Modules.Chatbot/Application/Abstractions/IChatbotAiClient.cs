using DTP.Modules.Chatbot.Application.DTOs;
using DTP.Modules.Knowledge.Application.DTOs;
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
        string message,
        ChatbotIntentDto intent,
        IReadOnlyList<ChatbotProductSuggestionDto> suggestions,
        CancellationToken cancellationToken = default);

        Task<string> GenerateKnowledgeAnswerAsync(
            string userMessage,
            IReadOnlyList<KnowledgeSearchResultDto> knowledgeResults,
            CancellationToken cancellationToken = default);

        Task<string> GenerateMixedAnswerAsync(
            string userMessage,
            ChatbotIntentDto intent,
            IReadOnlyList<ChatbotProductSuggestionDto> suggestions,
            IReadOnlyList<KnowledgeSearchResultDto> knowledgeResults,
            CancellationToken cancellationToken = default);
    }
}
