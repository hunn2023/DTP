using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Infrastructure.Options
{
    public sealed class OpenAiChatbotOptions
    {
        public string BaseUrl { get; set; } = "https://api.openai.com";

        public string ApiKey { get; set; } = string.Empty;

        public string Model { get; set; } = "gpt-4.1-mini";

        public int MaxOutputTokens { get; set; } = 800;

        public bool DisableAi { get; set; }
    }
}
