using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.Options
{
    public class KnowledgeOptions
    {
        public string StorefrontBaseUrl { get; set; } = "https://ezsim.vercel.app";

        public string ProductDetailPath { get; set; } = "esim-du-lich";

        public string ContentDetailPath { get; set; } = "blog";

        public int ChunkMaxChars { get; set; } = 1600;

        public int ChunkOverlapChars { get; set; } = 200;

        public int SearchCandidateLimit { get; set; } = 1000;

        public double MinScore { get; set; } = 0.72;
    }
}
