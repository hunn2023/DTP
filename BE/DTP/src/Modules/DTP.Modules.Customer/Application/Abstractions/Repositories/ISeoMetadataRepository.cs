using DTP.Modules.Content.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Repositories
{
    public interface ISeoMetadataRepository
    {
        Task<SeoMetadata?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<SeoMetadata?> GetByEntityAsync(
            string entityType,
            Guid entityId,
            CancellationToken cancellationToken = default);

        Task<SeoMetadata?> GetByRoutePathAsync(
            string routePath,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsEntityAsync(
            string entityType,
            Guid entityId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsRoutePathAsync(
            string routePath,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<SeoMetadata> Items, int TotalCount)> GetPagedAsync(
            string? keyword,
            string? entityType,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            SeoMetadata seo,
            CancellationToken cancellationToken = default);

        void Update(SeoMetadata seo);

        void Delete(SeoMetadata seo);
    }
}
