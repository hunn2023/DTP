using DTP.Modules.Chatbot.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.Abstractions
{
    public interface IChatbotCatalogReader
    {
        Task<IReadOnlyList<ChatbotProductSuggestionDto>> SearchEsimPackagesAsync(
            ChatbotIntentDto intent,
            int take = 3,
            CancellationToken cancellationToken = default);
    }
}
