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
    public class ContentPageRepository : IContentPageRepository
    {
        private readonly ContentDbContext _dbContext;

        public ContentPageRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ContentPage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.ContentPages
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<ContentPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLowerInvariant();

            return _dbContext.ContentPages
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        public Task<bool> ExistsCodeAsync(string code, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            code = code.Trim();

            return _dbContext.ContentPages
                .AnyAsync(x => x.Code == code && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
        }

        public Task<bool> ExistsSlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            slug = slug.Trim().ToLowerInvariant();

            return _dbContext.ContentPages
                .AnyAsync(x => x.Slug == slug && (!excludeId.HasValue || x.Id != excludeId.Value), cancellationToken);
        }

        public async Task<(IReadOnlyList<ContentPage> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            ContentPageStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ContentPages.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(x =>
                    x.Code.Contains(keyword) ||
                    x.Title.Contains(keyword) ||
                    x.Slug.Contains(keyword));
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<ContentPage>> GetPublishedAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ContentPages
                .AsNoTracking()
                .Where(x => x.Status == ContentPageStatus.Published)
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.PublishedAt)
                .ToListAsync(cancellationToken);
        }

        public Task AddAsync(ContentPage page, CancellationToken cancellationToken = default)
        {
            return _dbContext.ContentPages.AddAsync(page, cancellationToken).AsTask();
        }

        public void Update(ContentPage page)
        {
            _dbContext.ContentPages.Update(page);
        }
    }
}
