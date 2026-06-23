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
            var chunks = new List<KnowledgeChunk>();

            var productChunks = await BuildProductChunksAsync(null, cancellationToken);
            var productFaqChunks = await BuildProductFaqChunksAsync(null, null, cancellationToken);
            var contentChunks = await BuildContentChunksAsync(null, cancellationToken);
            var contentFaqChunks = await BuildContentFaqChunksAsync(null, null, cancellationToken);

            chunks.AddRange(productChunks.Chunks);
            chunks.AddRange(productFaqChunks.Chunks);
            chunks.AddRange(contentChunks.Chunks);
            chunks.AddRange(contentFaqChunks.Chunks);

            await CreateEmbeddingsAsync(chunks, cancellationToken);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await _dbContext.KnowledgeChunks.ExecuteDeleteAsync(cancellationToken);

            if (chunks.Count > 0)
            {
                await _dbContext.KnowledgeChunks.AddRangeAsync(chunks, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ReindexKnowledgeResultDto
            {
                ProductCount = productChunks.SourceCount,
                ProductFaqCount = productFaqChunks.SourceCount,
                ContentCount = contentChunks.SourceCount,
                ContentFaqCount = contentFaqChunks.SourceCount,
                ChunkCount = chunks.Count,
                IndexedAt = DateTime.UtcNow
            };
        }

        public async Task<ReindexKnowledgeResultDto> ReindexSourceAsync(
            KnowledgeSourceType sourceType,
            Guid sourceId,
            CancellationToken cancellationToken = default)
        {
            var chunks = new List<KnowledgeChunk>();

            switch (sourceType)
            {
                case KnowledgeSourceType.Product:
                    {
                        var productChunks = await BuildProductChunksAsync(
                            new List<Guid> { sourceId },
                            cancellationToken);

                        var productFaqChunks = await BuildProductFaqChunksAsync(
                            faqIds: null,
                            productIds: new List<Guid> { sourceId },
                            cancellationToken);

                        chunks.AddRange(productChunks.Chunks);
                        chunks.AddRange(productFaqChunks.Chunks);
                        break;
                    }

                case KnowledgeSourceType.ProductFaq:
                    {
                        var productFaqChunks = await BuildProductFaqChunksAsync(
                            faqIds: new List<Guid> { sourceId },
                            productIds: null,
                            cancellationToken);

                        chunks.AddRange(productFaqChunks.Chunks);
                        break;
                    }

                case KnowledgeSourceType.Content:
                    {
                        var contentChunks = await BuildContentChunksAsync(
                            new List<Guid> { sourceId },
                            cancellationToken);

                        var contentFaqChunks = await BuildContentFaqChunksAsync(
                            faqIds: null,
                            contentIds: new List<Guid> { sourceId },
                            cancellationToken);

                        chunks.AddRange(contentChunks.Chunks);
                        chunks.AddRange(contentFaqChunks.Chunks);
                        break;
                    }

                case KnowledgeSourceType.ContentFaq:
                    {
                        var contentFaqChunks = await BuildContentFaqChunksAsync(
                            faqIds: new List<Guid> { sourceId },
                            contentIds: null,
                            cancellationToken);

                        chunks.AddRange(contentFaqChunks.Chunks);
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(sourceType), sourceType, null);
            }

            await CreateEmbeddingsAsync(chunks, cancellationToken);

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await DeleteOldChunksAsync(sourceType, sourceId, cancellationToken);

            if (chunks.Count > 0)
            {
                await _dbContext.KnowledgeChunks.AddRangeAsync(chunks, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ReindexKnowledgeResultDto
            {
                ProductCount = chunks.Count(x => x.SourceType == KnowledgeSourceType.Product),
                ProductFaqCount = chunks.Count(x => x.SourceType == KnowledgeSourceType.ProductFaq),
                ContentCount = chunks.Count(x => x.SourceType == KnowledgeSourceType.Content),
                ContentFaqCount = chunks.Count(x => x.SourceType == KnowledgeSourceType.ContentFaq),
                ChunkCount = chunks.Count,
                IndexedAt = DateTime.UtcNow
            };
        }

        private async Task DeleteOldChunksAsync(
            KnowledgeSourceType sourceType,
            Guid sourceId,
            CancellationToken cancellationToken)
        {
            switch (sourceType)
            {
                case KnowledgeSourceType.Product:
                    {
                        var faqIds = await _dbContext.ProductFaqs
                            .AsNoTracking()
                            .Where(x => x.ProductId == sourceId)
                            .Select(x => x.Id)
                            .ToListAsync(cancellationToken);

                        await _dbContext.KnowledgeChunks
                            .Where(x =>
                                (x.SourceType == KnowledgeSourceType.Product && x.SourceId == sourceId) ||
                                (x.SourceType == KnowledgeSourceType.ProductFaq && faqIds.Contains(x.SourceId)))
                            .ExecuteDeleteAsync(cancellationToken);

                        break;
                    }

                case KnowledgeSourceType.ProductFaq:
                    {
                        await _dbContext.KnowledgeChunks
                            .Where(x => x.SourceType == KnowledgeSourceType.ProductFaq && x.SourceId == sourceId)
                            .ExecuteDeleteAsync(cancellationToken);

                        break;
                    }

                case KnowledgeSourceType.Content:
                    {
                        var faqIds = await _dbContext.ContentFaqs
                            .AsNoTracking()
                            .Where(x => x.ContentId == sourceId)
                            .Select(x => x.Id)
                            .ToListAsync(cancellationToken);

                        await _dbContext.KnowledgeChunks
                            .Where(x =>
                                (x.SourceType == KnowledgeSourceType.Content && x.SourceId == sourceId) ||
                                (x.SourceType == KnowledgeSourceType.ContentFaq && faqIds.Contains(x.SourceId)))
                            .ExecuteDeleteAsync(cancellationToken);

                        break;
                    }

                case KnowledgeSourceType.ContentFaq:
                    {
                        await _dbContext.KnowledgeChunks
                            .Where(x => x.SourceType == KnowledgeSourceType.ContentFaq && x.SourceId == sourceId)
                            .ExecuteDeleteAsync(cancellationToken);

                        break;
                    }
            }
        }

        private async Task CreateEmbeddingsAsync(
            List<KnowledgeChunk> chunks,
            CancellationToken cancellationToken)
        {
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
        }

        private async Task<(int SourceCount, List<KnowledgeChunk> Chunks)> BuildProductChunksAsync(
            List<Guid>? productIds,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Products
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted);

            if (productIds is { Count: > 0 })
            {
                query = query.Where(x => productIds.Contains(x.Id));
            }

            var products = await query.ToListAsync(cancellationToken);

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
            List<Guid>? faqIds,
            List<Guid>? productIds,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.ProductFaqs
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted);

            if (faqIds is { Count: > 0 })
            {
                query = query.Where(x => faqIds.Contains(x.Id));
            }

            if (productIds is { Count: > 0 })
            {
                query = query.Where(x => productIds.Contains(x.ProductId));
            }

            var faqs = await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);

            var parentProductIds = faqs
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var products = await _dbContext.Products
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted)
                .Where(x => parentProductIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var chunks = new List<KnowledgeChunk>();

            foreach (var faq in faqs)
            {
                if (!products.TryGetValue(faq.ProductId, out var product))
                    continue;

                var text = $"""
            Loại dữ liệu: Câu hỏi thường gặp về sản phẩm eSIM
            Sản phẩm: {product.Name}
            Câu hỏi: {faq.Question}
            Trả lời: {faq.Answer}
            """;

                chunks.Add(CreateChunk(
                    KnowledgeSourceType.ProductFaq,
                    faq.Id,
                    0,
                    product.Name,
                    product.Slug,
                    BuildProductUrl(product.Slug),
                    text));
            }

            return (faqs.Count, chunks);
        }

        private async Task<(int SourceCount, List<KnowledgeChunk> Chunks)> BuildContentChunksAsync(
            List<Guid>? contentIds,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Contents
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted && x.IsPublished);

            if (contentIds is { Count: > 0 })
            {
                query = query.Where(x => contentIds.Contains(x.Id));
            }

            var contents = await query.ToListAsync(cancellationToken);

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
            List<Guid>? faqIds,
            List<Guid>? contentIds,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.ContentFaqs
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted);

            if (faqIds is { Count: > 0 })
            {
                query = query.Where(x => faqIds.Contains(x.Id));
            }

            if (contentIds is { Count: > 0 })
            {
                query = query.Where(x => contentIds.Contains(x.ContentId));
            }

            var faqs = await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);

            var parentContentIds = faqs
                .Select(x => x.ContentId)
                .Distinct()
                .ToList();

            var contents = await _dbContext.Contents
                .AsNoTracking()
                .Where(x => x.IsActive && !x.IsDeleted && x.IsPublished)
                .Where(x => parentContentIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            var chunks = new List<KnowledgeChunk>();

            foreach (var faq in faqs)
            {
                if (!contents.TryGetValue(faq.ContentId, out var content))
                    continue;

                var text = $"""
            Loại dữ liệu: Câu hỏi thường gặp nội dung/hướng dẫn
            Bài viết: {content.Title}
            Câu hỏi: {faq.Question}
            Trả lời: {faq.Answer}
            """;

                chunks.Add(CreateChunk(
                    KnowledgeSourceType.ContentFaq,
                    faq.Id,
                    0,
                    content.Title,
                    content.Slug,
                    BuildContentUrl(content.Slug),
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
