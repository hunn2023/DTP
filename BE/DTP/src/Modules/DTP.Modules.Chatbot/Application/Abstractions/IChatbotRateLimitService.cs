using DTP.Modules.Chatbot.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.Abstractions
{
    public interface IChatbotRateLimitService
    {
        Task<ChatbotRateLimitResult> CheckAsync(
            ChatbotRateLimitContext context,
            CancellationToken cancellationToken = default);
    }
}
