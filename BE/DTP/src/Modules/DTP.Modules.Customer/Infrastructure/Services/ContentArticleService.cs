using DTP.Modules.Content.Application.Abstractions;
using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Application.Abstractions.Services;
using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Services
{
    public class ContentArticleService : IContentArticleService
    {
        private readonly IContentArticleRepository _repository;
        private readonly IContentUnitOfWork _unitOfWork;

        public ContentArticleService(
            IContentArticleRepository repository,
            IContentUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ContentArticleDto> CreateAsync(
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
                throw new Exception("Article slug already exists.");

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

            return Map(article);
        }

        public async Task<ContentArticleDto> UpdateAsync(
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
                throw new Exception("Article not found.");

            ValidateArticle(title, slug, content, summary);

            if (await _repository.ExistsSlugAsync(slug, id, cancellationToken))
                throw new Exception("Article slug already exists.");

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

            return Map(article);
        }

        public async Task<bool> PublishAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                throw new Exception("Article not found.");

            article.Publish();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> HideAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                throw new Exception("Article not found.");

            article.Hide();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> MarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                throw new Exception("Article not found.");

            article.MarkAsFeatured();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UnmarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            if (article == null)
                throw new Exception("Article not found.");

            article.UnmarkAsFeatured();

            _repository.Update(article);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<ContentArticleDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetByIdAsync(id, cancellationToken);

            return article == null ? null : Map(article);
        }

        public async Task<ContentArticleDto?> GetBySlugAsync(
            string slug,
            bool increaseView,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return null;

            var article = await _repository.GetBySlugAsync(slug, cancellationToken);

            if (article == null)
                return null;

            if (!article.IsPublished)
                return null;

            if (increaseView)
            {
                article.IncreaseView();
                _repository.Update(article);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            return Map(article);
        }

        public async Task<PagedResultDto<ContentArticleListItemDto>> GetPagedAsync(
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

            return new PagedResultDto<ContentArticleListItemDto>(
                result.Items.Select(MapListItem).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);
        }

        public async Task<PagedResultDto<ContentArticleListItemDto>> GetPublicPagedAsync(
            string? keyword,
            string? categoryCode,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            categoryCode = NormalizeCategoryCode(categoryCode);

            var result = await _repository.GetPublicPagedAsync(
                keyword,
                categoryCode,
                pageIndex,
                pageSize,
                cancellationToken);

            return new PagedResultDto<ContentArticleListItemDto>(
                result.Items.Select(MapListItem).ToList(),
                result.TotalCount,
                pageIndex,
                pageSize);
        }

        public async Task<IReadOnlyList<ContentArticleListItemDto>> GetFeaturedAsync(
            int take,
            CancellationToken cancellationToken = default)
        {
            if (take <= 0)
                take = 6;

            if (take > 20)
                take = 20;

            var articles = await _repository.GetFeaturedAsync(
                take,
                cancellationToken);

            return articles.Select(MapListItem).ToList();
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
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt,
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
    }
}
