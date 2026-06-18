using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Domain
{
    public sealed class KnowledgeChunk
    {
        public Guid Id { get; set; }

        public string SourceType { get; set; } = string.Empty;
        // Content | ContentFaq | ProductContent | ProductFaq

        public Guid SourceId { get; set; }

        public Guid? ProductId { get; set; }
        // Content, ContentFaq => null
        // ProductContent, ProductFaq => có ProductId

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int ChunkIndex { get; set; }

        public string Category { get; set; } = string.Empty;
        // setup_guide | policy | faq | product_content | product_faq | troubleshooting

        public string Language { get; set; } = "vi";

        public string EmbeddingModel { get; set; } = string.Empty;

        public string EmbeddingJson { get; set; } = string.Empty;

        public string ContentHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
