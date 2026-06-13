using DTP.Modules.Content.Application.Abstractions;
using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;
using DTP.Shared.Storage;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Content.Infrastructure.Services
{
    public class ContentArticleService : IContentArticleService
    {
        private readonly IContentArticleRepository _repository;
        private readonly IContentUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IFileStorageService _fileStorageService;
        public const string ContentArticleThumbnails = "content/articles/thumbnails";
        public ContentArticleService(
            IContentArticleRepository repository,
            IContentUnitOfWork unitOfWork,
            ICacheService cacheService,
            IFileStorageService fileStorageService
            )
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _fileStorageService = fileStorageService;
        }

        public async Task<Result<ContentArticleDto>> CreateAsync(
            string title,
            string slug,
            string? summary,
            string content,
            string? thumbnailUrl,
            string? authorName,
            string? categoryCode,
            string? tags,
            ContentArticleStatus status,
            bool isFeatured,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            ValidateArticle(title, slug, content, summary);

            if (await _repository.ExistsSlugAsync(slug, null, cancellationToken))
                return Result<ContentArticleDto>.Failure("Slug already exists.");


            var article = new ContentArticle(
                title,
                slug,
                summary,
                content,
                thumbnailUrl,
                authorName,
                categoryCode,
                tags,
                status,
                isFeatured,
                sortOrder);

            await _repository.AddAsync(article, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await RemoveArticleCacheAsync(cancellationToken);
            return Result<ContentArticleDto>.Success(Map(article));

        }

        public async Task<Result<ContentArticleDto>> UpdateAsync(
            Guid id,
            string title,
            string slug,
            string? summary,
            string content,
            string? thumbnailUrl,
            string? authorName,
            string? categoryCode,
            string? tags,
            ContentArticleStatus status,
            bool isFeatured,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                return Result<ContentArticleDto>.Failure("Article not found.");

            ValidateArticle(title, slug, content, summary);

            if (await _repository.ExistsSlugAsync(slug, id, cancellationToken))
                return Result<ContentArticleDto>.Failure("Slug already exists.");


            article.Update(
                title,
                slug,
                summary,
                content,
                thumbnailUrl,
                authorName,
                categoryCode,
                tags,
                status,
                isFeatured,
                sortOrder);

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await RemoveArticleCacheAsync(cancellationToken);

            return Result<ContentArticleDto>.Success(Map(article));
        }

        public async Task<Result<ContentArticleDto>> UploadThumbnailAsync(
            Guid articleId,
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            if (articleId == Guid.Empty)
                return Result<ContentArticleDto>.Failure("Id bài viết không hợp lệ.");

            if (file == null || file.Length == 0)
                return Result<ContentArticleDto>.Failure("Vui lòng chọn file ảnh.");

            var article = await _repository.GetByIdAsync(
                articleId,
                cancellationToken);

            if (article == null)
                return Result<ContentArticleDto>.Failure("Không tìm thấy bài viết.");

            var uploadResult = await _fileStorageService.UploadImageAsync(
                file,
                ContentArticleThumbnails,
                cancellationToken);

            article.UpdateThumbnail(
                thumbnailUrl: uploadResult.Url,
                thumbnailKey: uploadResult.Key);

            _repository.Update(article);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await RemoveArticleCacheAsync(cancellationToken);

            return Result<ContentArticleDto>.Success(Map(article));
        }


        public async Task<Result> PublishAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                return Result.Failure("Article not found.");

            article.Publish();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await RemoveArticleCacheAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> HideAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                return Result.Failure("Article not found.");

            article.Hide();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await RemoveArticleCacheAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> MarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                return Result.Failure("Article not found.");

            //article.MarkAsFeatured();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> UnmarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                return Result.Failure("Article not found.");

            //article.UnmarkAsFeatured();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<ContentArticleDto?>> GetByIdAsync(
           Guid id,
           CancellationToken cancellationToken = default)
        {
            var cacheKey = CacheKeys.DetailById(id);

            var cached = await _cacheService.GetAsync<ContentArticleDto?>(cacheKey, cancellationToken);

            if (cached != null)
                return Result<ContentArticleDto?>.Success(cached);


            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null) return Result<ContentArticleDto?>.Success(null);


            var dto = Map(article);

            await _cacheService.SetAsync(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<ContentArticleDto?>.Success(dto);
        }

        public async Task<Result<ContentArticleDto?>> GetBySlugAsync(
             string slug,
             bool increaseView,
             CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Result<ContentArticleDto?>.Success(null);

            slug = slug.Trim();

            if (!increaseView)
            {
                var cacheKey = CacheKeys.DetailBySlug(slug);

                var cached = await _cacheService.GetAsync<ContentArticleDto?>(cacheKey, cancellationToken);

                if (cached != null)
                    return Result<ContentArticleDto?>.Success(cached);

            }

            var article = await _repository.GetBySlugAsync(slug, cancellationToken);

            if (article == null) return Result<ContentArticleDto?>.Success(null);


            if (!article.IsPublished)
                return Result<ContentArticleDto?>.Success(null);

            if (increaseView)
            {
                article.IncreaseView();

                _repository.Update(article);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await RemoveArticleCacheAsync(cancellationToken);
            }

            var dto = Map(article);

            if (!increaseView)
            {
                await _cacheService.SetAsync(
                    CacheKeys.DetailBySlug(slug),
                    dto,
                    TimeSpan.FromMinutes(10),
                    cancellationToken);
            }

            return Result<ContentArticleDto?>.Success(dto);
        }

        public async Task<Result<PagedResultDto<ContentArticleListItemDto>>> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            ContentArticleStatus? status,
            bool? isFeatured,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            categoryCode = NormalizeCategoryCode(categoryCode);

            var result = await _repository.GetPagedAsync(
                keyword,
                categoryCode,
                status,
                isFeatured,
                pageIndex,
                pageSize,
                cancellationToken);


            return Result<PagedResultDto<ContentArticleListItemDto>>.Success(new PagedResultDto<ContentArticleListItemDto>(
                 result.Items.Select(MapListItem).ToList(),
                 result.TotalCount,
                 pageIndex,
                 pageSize));

        }

        public async Task<Result<PagedResultDto<ContentArticleListItemDto>>> GetPublicPagedAsync(
            string? keyword,
            string? categoryCode,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            categoryCode = NormalizeCategoryCode(categoryCode);

            var cacheKey = CacheKeys.PublicPaged(
                keyword,
                categoryCode,
                pageIndex,
                pageSize);

            var cached = await _cacheService.GetAsync<PagedResultDto<ContentArticleListItemDto>>(
                cacheKey,
                cancellationToken);

            if (cached != null) return Result<PagedResultDto<ContentArticleListItemDto>>.Success(cached);

            var result = await _repository.GetPublicPagedAsync(
                keyword,
                categoryCode,
                pageIndex,
                pageSize,
                cancellationToken);

            var dto = new PagedResultDto<ContentArticleListItemDto>(
                result.Items.Select(MapListItem).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);

            await _cacheService.SetAsync(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(5),
                cancellationToken);

            return Result<PagedResultDto<ContentArticleListItemDto>>.Success(dto);
        }

        public async Task<Result<IReadOnlyList<ContentArticleListItemDto>>> GetFeaturedAsync(
             int take,
             CancellationToken cancellationToken = default)
        {
            if (take <= 0)
                take = 6;

            if (take > 20)
                take = 20;

            var cacheKey = CacheKeys.Featured(take);

            var cached = await _cacheService.GetAsync<IReadOnlyList<ContentArticleListItemDto>>(
                cacheKey,
                cancellationToken);

            if (cached != null) return Result<IReadOnlyList<ContentArticleListItemDto>>.Success(cached);


            var articles = await _repository.GetFeaturedAsync(
                take,
                cancellationToken);

            var dto = articles.Select(MapListItem).ToList();

            await _cacheService.SetAsync(
                cacheKey,
                dto,
                TimeSpan.FromMinutes(10),
                cancellationToken);

            return Result<IReadOnlyList<ContentArticleListItemDto>>.Success(dto);
        }

        private static void ValidateArticle(
            string title,
            string slug,
            string content,
            string? summary)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new Exception("Article title is required.");

            if (title.Trim().Length > 255)
                throw new Exception("Article title cannot exceed 255 characters.");

            if (string.IsNullOrWhiteSpace(slug))
                throw new Exception("Article slug is required.");

            if (slug.Trim().Length > 255)
                throw new Exception("Article slug cannot exceed 255 characters.");

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Article content is required.");

            if (!string.IsNullOrWhiteSpace(summary) && summary.Length > 1000)
                throw new Exception("Article summary cannot exceed 1000 characters.");
        }

        private static void NormalizePaging(
            ref int pageIndex,
            ref int pageSize)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            if (pageSize > 100)
                pageSize = 100;
        }

        private static string? NormalizeCategoryCode(string? categoryCode)
        {
            return string.IsNullOrWhiteSpace(categoryCode)
                ? null
                : categoryCode.Trim().ToUpperInvariant();
        }

        private static ContentArticleDto Map(ContentArticle article)
        {
            return new ContentArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Summary = article.Summary,
                Content = article.Content,
                ThumbnailUrl = article.ThumbnailUrl,
                AuthorName = article.AuthorName,
                CategoryCode = article.CategoryCode,
                Tags = article.Tags,
                Status = article.Status,
                IsFeatured = article.IsFeatured,
                SortOrder = article.SortOrder,
                ViewCount = article.ViewCount,
                PublishedAt = article.PublishedAt
            };
        }

        private static ContentArticleListItemDto MapListItem(ContentArticle article)
        {
            return new ContentArticleListItemDto
            {
                Id = article.Id,
                Title = article.Title,
                Slug = article.Slug,
                Summary = article.Summary,
                ThumbnailUrl = article.ThumbnailUrl,
                AuthorName = article.AuthorName,
                CategoryCode = article.CategoryCode,
                Tags = article.Tags,
                Status = article.Status,
                IsFeatured = article.IsFeatured,
                SortOrder = article.SortOrder,
                ViewCount = article.ViewCount,
                CreatedAt = article.CreatedAt,
                PublishedAt = article.PublishedAt
            };
        }

        private async Task RemoveArticleCacheAsync(CancellationToken cancellationToken)
        {
            await _cacheService.RemoveByPrefixAsync(CacheKeys.Prefix, cancellationToken);
        }
        private static class CacheKeys
        {
            public const string Prefix = "content:article";

            public static string DetailById(Guid id)
                => $"{Prefix}:detail:id:{id}";

            public static string DetailBySlug(string slug)
                => $"{Prefix}:detail:slug:{slug.Trim().ToLowerInvariant()}";

            public static string AdminPaged(
                string? keyword,
                string? categoryCode,
                ContentArticleStatus? status,
                bool? isFeatured,
                int pageIndex,
                int pageSize)
                => $"{Prefix}:admin:paged:{Normalize(keyword)}:{Normalize(categoryCode)}:{status}:{isFeatured}:{pageIndex}:{pageSize}";

            public static string PublicPaged(
                string? keyword,
                string? categoryCode,
                int pageIndex,
                int pageSize)
                => $"{Prefix}:public:paged:{Normalize(keyword)}:{Normalize(categoryCode)}:{pageIndex}:{pageSize}";

            public static string Featured(int take)
                => $"{Prefix}:featured:{take}";

            private static string Normalize(string? value)
                => string.IsNullOrWhiteSpace(value)
                    ? "all"
                    : value.Trim().ToLowerInvariant();
        }
    }
}
