using DTP.Modules.Knowledge.Application.Abstractions;
using DTP.Modules.Knowledge.Application.DTOs;
using DTP.Modules.Knowledge.Application.Options;
using DTP.Modules.Knowledge.Domain.Entities;
using DTP.Modules.Knowledge.Domain.Enums;
using DTP.Modules.Knowledge.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Knowledge.Infrastructure.Services
{
    public class KnowledgeIndexerService : IKnowledgeIndexerService
    {
        private readonly KnowledgeDbContext _dbContext;
        private readonly ITextChunkerService _chunker;
        private readonly IEmbeddingService _embeddingService;
        private readonly KnowledgeOptions _options;
        private readonly KnowledgeOpenAiOptions _openAiOptions;

        public KnowledgeIndexerService(
            KnowledgeDbContext dbContext,
            ITextChunkerService chunker,
            IEmbeddingService embeddingService,
            IOptions<KnowledgeOptions> options,
            IOptions<KnowledgeOpenAiOptions> openAiOptions)
        {
            _dbContext = dbContext;
            _chunker = chunker;
            _embeddingService = embeddingService;
            _options = options.Value;
            _openAiOptions = openAiOptions.Value;
        }

        public async Task<ReindexKnowledgeResultDto> ReindexAllAsync(
            CancellationToken cancellationToken = default)
        {
            var result = new ReindexKnowledgeResultDto();

            var chunks = new List<KnowledgeChunk>();

            var productChunks = await BuildProductChunksAsync(cancellationToken);
            chunks.AddRange(productChunks.Chunks);
            result.ProductCount = productChunks.SourceCount;

            var productFaqChunks = await BuildProductFaqChunksAsync(cancellationToken);
            chunks.AddRange(productFaqChunks.Chunks);
            result.ProductFaqCount = productFaqChunks.SourceCount;

            var contentChunks = await BuildContentChunksAsync(cancellationToken);
            chunks.AddRange(contentChunks.Chunks);
            result.ContentCount = contentChunks.SourceCount;

            var contentFaqChunks = await BuildContentFaqChunksAsync(cancellationToken);
            chunks.AddRange(contentFaqChunks.Chunks);
            result.ContentFaqCount = contentFaqChunks.SourceCount;

            foreach (var chunk in chunks)
            {
                var embedding = await _embeddingService.CreateEmbeddingAsync(
                    chunk.Content,
                    cancellationToken);

                chunk.SetEmbedding(
                    JsonSerializer.Serialize(embedding),
                    _openAiOptions.EmbeddingModel,
                    embedding.Length);
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.KnowledgeChunks.ExecuteDeleteAsync(cancellationToken);

            await _dbContext.KnowledgeChunks.AddRangeAsync(chunks, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            result.ChunkCount = chunks.Count;
            result.IndexedAt = DateTime.UtcNow;

            return result;
        }

        private async Task<(int SourceCount, List<KnowledgeChunk> Chunks)> BuildProductChunksAsync(
            CancellationToken cancellationToken)
        {
            var products = await _dbContext.Products
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            var chunks = new List<KnowledgeChunk>();

            foreach (var product in products)
            {
                var text = $"""
            Loại dữ liệu: Sản phẩm eSIM
            Tên sản phẩm: {product.Name}
            Điểm đến/khu vực: {product.LocationText}
            Mô tả ngắn: {product.ShortDescription}
            Nội dung: {product.Description}
            """;

                var sourceUrl = BuildProductUrl(product.Slug);

                var parts = _chunker.Split(
                    text,
                    _options.ChunkMaxChars,
                    _options.ChunkOverlapChars);

                for (var i = 0; i < parts.Count; i++)
                {
                    chunks.Add(CreateChunk(
                        KnowledgeSourceType.Product,
                        product.Id,
                        i,
                        product.Name,
                        product.Slug,
                        sourceUrl,
                        parts[i]));
                }
            }

            return (products.Count, chunks);
        }

        private async Task<(int SourceCount, List<KnowledgeChunk> Chunks)> BuildProductFaqChunksAsync(
            CancellationToken cancellationToken)
        {
            var faqs = await _dbContext.ProductFaqs
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);

            var productIds = faqs.Select(x => x.ProductId).Distinct().ToList();

            var products = await _dbContext.Products
                .AsNoTracking()
                .Where(x => productIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var chunks = new List<KnowledgeChunk>();

            foreach (var faq in faqs)
            {
                products.TryGetValue(faq.ProductId, out var product);

                var title = product?.Name ?? "FAQ sản phẩm";
                var slug = product?.Slug;
                var sourceUrl = slug == null ? null : BuildProductUrl(slug);

                var text = $"""
            Loại dữ liệu: Câu hỏi thường gặp về sản phẩm eSIM
            Sản phẩm: {title}
            Câu hỏi: {faq.Question}
            Trả lời: {faq.Answer}
            """;

                chunks.Add(CreateChunk(
                    KnowledgeSourceType.ProductFaq,
                    faq.Id,
                    0,
                    title,
                    slug,
                    sourceUrl,
                    text));
            }

            return (faqs.Count, chunks);
        }

        private async Task<(int SourceCount, List<KnowledgeChunk> Chunks)> BuildContentChunksAsync(
            CancellationToken cancellationToken)
        {
            var contents = await _dbContext.Contents
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted && x.IsPublished)
                .ToListAsync(cancellationToken);

            var chunks = new List<KnowledgeChunk>();

            foreach (var content in contents)
            {
                var text = $"""
            Loại dữ liệu: Nội dung hướng dẫn/bài viết
            Tiêu đề: {content.Title}
            Tóm tắt: {content.Summary}
            Nội dung: {content.Body}
            """;

                var sourceUrl = BuildContentUrl(content.Slug);

                var parts = _chunker.Split(
                    text,
                    _options.ChunkMaxChars,
                    _options.ChunkOverlapChars);

                for (var i = 0; i < parts.Count; i++)
                {
                    chunks.Add(CreateChunk(
                        KnowledgeSourceType.Content,
                        content.Id,
                        i,
                        content.Title,
                        content.Slug,
                        sourceUrl,
                        parts[i]));
                }
            }

            return (contents.Count, chunks);
        }

        private async Task<(int SourceCount, List<KnowledgeChunk> Chunks)> BuildContentFaqChunksAsync(
            CancellationToken cancellationToken)
        {
            var faqs = await _dbContext.ContentFaqs
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);

            var contentIds = faqs.Select(x => x.ContentId).Distinct().ToList();

            var contents = await _dbContext.Contents
                .AsNoTracking()
                .Where(x => contentIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var chunks = new List<KnowledgeChunk>();

            foreach (var faq in faqs)
            {
                contents.TryGetValue(faq.ContentId, out var content);

                var title = content?.Title ?? "FAQ nội dung";
                var slug = content?.Slug;
                var sourceUrl = slug == null ? null : BuildContentUrl(slug);

                var text = $"""
            Loại dữ liệu: Câu hỏi thường gặp nội dung/hướng dẫn
            Bài viết: {title}
            Câu hỏi: {faq.Question}
            Trả lời: {faq.Answer}
            """;

                chunks.Add(CreateChunk(
                    KnowledgeSourceType.ContentFaq,
                    faq.Id,
                    0,
                    title,
                    slug,
                    sourceUrl,
                    text));
            }

            return (faqs.Count, chunks);
        }

        private KnowledgeChunk CreateChunk(
            KnowledgeSourceType sourceType,
            Guid sourceId,
            int chunkIndex,
            string title,
            string? slug,
            string? sourceUrl,
            string content)
        {
            return new KnowledgeChunk(
                sourceType,
                sourceId,
                chunkIndex,
                title,
                slug,
                sourceUrl,
                content,
                ComputeSha256(content),
                "vi");
        }

        private string BuildProductUrl(string slug)
        {
            return $"{_options.StorefrontBaseUrl.TrimEnd('/')}/{_options.ProductDetailPath.Trim('/')}/{slug}";
        }

        private string BuildContentUrl(string slug)
        {
            return $"{_options.StorefrontBaseUrl.TrimEnd('/')}/{_options.ContentDetailPath.Trim('/')}/{slug}";
        }

        private static string ComputeSha256(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
