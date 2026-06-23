using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.DTOs;
using DTP.Modules.Knowledge.Application.Options;
using DTP.Modules.Knowledge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Services
{
    public class KnowledgeSearchService : IKnowledgeSearchService
    {
        private readonly IKnowledgeDbContext _dbContext;
        private readonly IEmbeddingService _embeddingService;
        private readonly KnowledgeOptions _options;

        public KnowledgeSearchService(
            IKnowledgeDbContext dbContext,
            IEmbeddingService embeddingService,
            IOptions<KnowledgeOptions> options)
        {
            _dbContext = dbContext;
            _embeddingService = embeddingService;
            _options = options.Value;
        }

        public async Task<List<KnowledgeSearchResultDto>> SearchAsync(
            string query,
            int topK = 5,
            CancellationToken cancellationToken = default)
        {
            query = query?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(query))
                return new List<KnowledgeSearchResultDto>();

            topK = Math.Clamp(topK, 1, 20);

            var queryEmbedding = await _embeddingService.CreateEmbeddingAsync(
                query,
                cancellationToken);

            var candidates = await _dbContext.KnowledgeChunks
                .AsNoTracking()
                .Where(x => x.IsActive && x.EmbeddingJson != null)
                .OrderByDescending(x => x.UpdatedAt)
                .Take(_options.SearchCandidateLimit)
                .Select(x => new
                {
                    x.Id,
                    x.SourceType,
                    x.SourceId,
                    x.Title,
                    x.Slug,
                    x.SourceUrl,
                    x.Content,
                    x.EmbeddingJson
                })
                .ToListAsync(cancellationToken);

            var results = new List<KnowledgeSearchResultDto>();

            foreach (var item in candidates)
            {
                var embedding = JsonSerializer.Deserialize<float[]>(item.EmbeddingJson!);

                if (embedding == null || embedding.Length == 0)
                    continue;

                var score = CosineSimilarity(queryEmbedding, embedding);

                score += KeywordBoost(query, item.Content);

                if (score < _options.MinScore)
                    continue;

                results.Add(new KnowledgeSearchResultDto
                {
                    Id = item.Id,
                    SourceType = item.SourceType,
                    SourceId = item.SourceId,
                    Title = item.Title,
                    Slug = item.Slug,
                    SourceUrl = item.SourceUrl,
                    Content = item.Content,
                    Score = Math.Round(score, 4)
                });
            }

            return results
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();
        }

        private static double CosineSimilarity(float[] a, float[] b)
        {
            if (a.Length != b.Length)
                return 0;

            double dot = 0;
            double normA = 0;
            double normB = 0;

            for (var i = 0; i < a.Length; i++)
            {
                dot += a[i] * b[i];
                normA += a[i] * a[i];
                normB += b[i] * b[i];
            }

            if (normA == 0 || normB == 0)
                return 0;

            return dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }

        private static double KeywordBoost(string query, string content)
        {
            var q = query.ToLowerInvariant();
            var c = content.ToLowerInvariant();

            var words = q
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => x.Length >= 3)
                .Distinct()
                .ToList();

            if (words.Count == 0)
                return 0;

            var matched = words.Count(c.Contains);

            return Math.Min(0.06, matched * 0.01);
        }
    }
}
