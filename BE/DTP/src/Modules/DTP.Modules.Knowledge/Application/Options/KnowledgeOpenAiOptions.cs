using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Options
{
    public class KnowledgeOpenAiOptions
    {
        public string BaseUrl { get; set; } = "https://api.openai.com";

        public string ApiKey { get; set; } = string.Empty;

        public string EmbeddingModel { get; set; } = "text-embedding-3-small";

        public int TimeoutSeconds { get; set; } = 60;
    }
}
