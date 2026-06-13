using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Domain.Enums;
using DTP.Modules.Content.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;


namespace DTP.Modules.Content.Infrastructure.Repositories
{
    public class ContentBannerRepository :  RepositoryBase<ContentBanner>, IContentBannerRepository

    {
        private readonly ContentDbContext _dbContext;

        public ContentBannerRepository(ContentDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<(IReadOnlyList<ContentBanner> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            ContentBannerPosition? position,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ContentBanners
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Title.Contains(keyword) ||
                    (x.Description != null && x.Description.Contains(keyword)) ||
                    (x.LinkUrl != null && x.LinkUrl.Contains(keyword)));
            }

            if (position.HasValue)
            {
                query = query.Where(x => x.Position == position.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
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

        public async Task<IReadOnlyList<ContentBanner>> GetAvailableAsync(
            ContentBannerPosition? position,
            DateTime now,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ContentBanners
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Where(x => !x.StartDate.HasValue || x.StartDate.Value <= now)
                .Where(x => !x.EndDate.HasValue || x.EndDate.Value >= now);

            if (position.HasValue)
            {
                query = query.Where(x => x.Position == position.Value);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
