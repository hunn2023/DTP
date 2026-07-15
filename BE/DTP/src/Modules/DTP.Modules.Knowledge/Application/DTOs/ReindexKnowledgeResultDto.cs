using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Application.DTOs
{
    public class ReindexKnowledgeResultDto
    {
        public int ProductCount { get; set; }

        public int ProductContentCount { get; set; }

        public int ProductFaqCount { get; set; }

        public int ContentCount { get; set; }

        public int ContentFaqCount { get; set; }

        public int ChunkCount { get; set; }

        public DateTime IndexedAt { get; set; } = DateTime.UtcNow;
    }
}
