using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.DTOs
{
    public sealed class ChatbotRateLimitContext
    {
        public string? IpAddress { get; set; }

        public string? UserId { get; set; }

        public string? SessionId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
