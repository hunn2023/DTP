using DTP.Modules.Knowledge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Domain.Entities
{
    public class KnowledgeChunk
    {
        public Guid Id { get; private set; }

        public KnowledgeSourceType SourceType { get; private set; }

        public Guid SourceId { get; private set; }

        public int ChunkIndex { get; private set; }

        public string Title { get; private set; } = string.Empty;

        public string? Slug { get; private set; }

        public string? SourceUrl { get; private set; }

        public string Content { get; private set; } = string.Empty;

        public string ContentHash { get; private set; } = string.Empty;

        public string? EmbeddingJson { get; private set; }

        public string? EmbeddingModel { get; private set; }

        public int? EmbeddingDimensions { get; private set; }

        public string LanguageCode { get; private set; } = "vi";

        public bool IsActive { get; private set; } = true;

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        private KnowledgeChunk()
        {
        }

        public KnowledgeChunk(
            KnowledgeSourceType sourceType,
            Guid sourceId,
            int chunkIndex,
            string title,
            string? slug,
            string? sourceUrl,
            string content,
            string contentHash,
            string languageCode = "vi")
        {
            Id = Guid.NewGuid();
            SourceType = sourceType;
            SourceId = sourceId;
            ChunkIndex = chunkIndex;
            Title = title.Trim();
            Slug = slug?.Trim();
            SourceUrl = sourceUrl?.Trim();
            Content = content.Trim();
            ContentHash = contentHash;
            LanguageCode = string.IsNullOrWhiteSpace(languageCode) ? "vi" : languageCode.Trim();

            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetEmbedding(string embeddingJson, string model, int dimensions)
        {
            EmbeddingJson = embeddingJson;
            EmbeddingModel = model;
            EmbeddingDimensions = dimensions;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
