using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.DTOs
{
    public sealed class ChatResponseDto
    {
        public string SessionId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool NeedMoreInfo { get; set; }

        public List<string> MissingFields { get; set; } = new();

        public ChatbotIntentDto? Intent { get; set; }

        public List<ChatbotProductSuggestionDto> Suggestions { get; set; } = new();
    }
}
