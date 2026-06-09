using DTP.Modules.Content.Application.Abstractions.Repositories;
using DTP.Modules.Content.Domain.Entities;
using DTP.Modules.Content.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Infrastructure.Repositories
{
    public class SeoMetadataRepository : ISeoMetadataRepository
    {
        private readonly ContentDbContext _dbContext;

        public SeoMetadataRepository(ContentDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<SeoMetadata?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.SeoMetadata
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public Task<SeoMetadata?> GetByEntityAsync(
            string entityType,
            Guid entityId,
            CancellationToken cancellationToken = default)
        {
            entityType = entityType.Trim();

            return _dbContext.SeoMetadata
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.EntityType == entityType &&
                         x.EntityId == entityId,
                    cancellationToken);
        }

        public Task<SeoMetadata?> GetByRoutePathAsync(
            string routePath,
            CancellationToken cancellationToken = default)
        {
            routePath = NormalizeRoutePath(routePath);

            return _dbContext.SeoMetadata
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.RoutePath == routePath,
                    cancellationToken);
        }

        public Task<bool> ExistsEntityAsync(
            string entityType,
            Guid entityId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            entityType = entityType.Trim();

            return _dbContext.SeoMetadata
                .AnyAsync(
                    x => x.EntityType == entityType &&
                         x.EntityId == entityId &&
                         (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public Task<bool> ExistsRoutePathAsync(
            string routePath,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            routePath = NormalizeRoutePath(routePath);

            return _dbContext.SeoMetadata
                .AnyAsync(
                    x => x.RoutePath == routePath &&
                         (!excludeId.HasValue || x.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task<(IReadOnlyList<SeoMetadata> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            string? entityType,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.SeoMetadata
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.EntityType.Contains(keyword) ||
                    (x.RoutePath != null && x.RoutePath.Contains(keyword)) ||
                    x.MetaTitle.Contains(keyword) ||
                    (x.MetaDescription != null && x.MetaDescription.Contains(keyword)) ||
                    (x.MetaKeywords != null && x.MetaKeywords.Contains(keyword)) ||
                    (x.CanonicalUrl != null && x.CanonicalUrl.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                entityType = entityType.Trim();

                query = query.Where(x => x.EntityType == entityType);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.EntityType)
                .ThenBy(x => x.RoutePath)
                .ThenByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public Task AddAsync(
            SeoMetadata seo,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.SeoMetadata
                .AddAsync(seo, cancellationToken)
                .AsTask();
        }

        public void Update(SeoMetadata seo)
        {
            _dbContext.SeoMetadata.Update(seo);
        }

        public void Delete(SeoMetadata seo)
        {
            _dbContext.SeoMetadata.Remove(seo);
        }

        private static string NormalizeRoutePath(string routePath)
        {
            routePath = routePath.Trim();

            if (!routePath.StartsWith("/"))
                routePath = "/" + routePath;

            return routePath.ToLowerInvariant();
        }
    }
}
