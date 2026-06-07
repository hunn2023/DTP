using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Modules.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Repositories
{
    public class ContentArticleRepository : IContentArticleRepository
    {
        private readonly ContentDbContext _dbContext;

        public ContentArticleRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ContentArticle?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ContentArticles
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<ContentArticle?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLowerInvariant();

            return _dbContext.ContentArticles
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        public Task<bool> ExistsSlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLowerInvariant();

            return _dbContext.ContentArticles
                .AnyAsync(
                    x => x.Slug == slug &&
                         (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task<(IReadOnlyList<ContentArticle> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            ContentArticleStatus? status,
            bool? isFeatured,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ContentArticles
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Title.Contains(keyword) ||
                    x.Slug.Contains(keyword) ||
                    (x.Summary != null && x.Summary.Contains(keyword)) ||
                    (x.AuthorName != null && x.AuthorName.Contains(keyword)) ||
                    (x.CategoryCode != null && x.CategoryCode.Contains(keyword)) ||
                    (x.Tags != null && x.Tags.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(categoryCode))
            {
                categoryCode = categoryCode.Trim().ToUpperInvariant();

                query = query.Where(x => x.CategoryCode == categoryCode);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (isFeatured.HasValue)
            {
                query = query.Where(x => x.IsFeatured == isFeatured.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.PublishedAt)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<(IReadOnlyList<ContentArticle> Items, int TotalCount)> GetPublicPagedAsync(
            string? keyword,
            string? categoryCode,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ContentArticles
                .AsNoTracking()
                .Where(x => x.Status == ContentArticleStatus.Published)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Title.Contains(keyword) ||
                    (x.Summary != null && x.Summary.Contains(keyword)) ||
                    (x.Tags != null && x.Tags.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(categoryCode))
            {
                categoryCode = categoryCode.Trim().ToUpperInvariant();

                query = query.Where(x => x.CategoryCode == categoryCode);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.PublishedAt)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<ContentArticle>> GetFeaturedAsync(
            int take,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ContentArticles
                .AsNoTracking()
                .Where(x => x.Status == ContentArticleStatus.Published)
                .Where(x => x.IsFeatured)
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.PublishedAt)
                .ThenByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public Task AddAsync(
            ContentArticle article,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ContentArticles
                .AddAsync(article, cancellationToken)
                .AsTask();
        }

        public void Update(ContentArticle article)
        {
            _dbContext.ContentArticles.Update(article);
        }
    }
}
