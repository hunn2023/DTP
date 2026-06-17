using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.DTOs
{
    public sealed class ChatbotRateLimitResult
    {
        public bool Allowed { get; set; }

        public string? Reason { get; set; }

        public string Message { get; set; } = string.Empty;

        public int? RetryAfterSeconds { get; set; }

        public static ChatbotRateLimitResult Success()
        {
            return new ChatbotRateLimitResult
            {
                Allowed = true
            };
        }

        public static ChatbotRateLimitResult Failed(
            string reason,
            string message,
            int? retryAfterSeconds = null)
        {
            return new ChatbotRateLimitResult
            {
                Allowed = false,
                Reason = reason,
                Message = message,
                RetryAfterSeconds = retryAfterSeconds
            };
        }
    }
}
