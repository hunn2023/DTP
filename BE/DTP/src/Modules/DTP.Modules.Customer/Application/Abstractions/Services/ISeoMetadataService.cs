using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Services
{
    public interface ISeoMetadataService
    {
        Task<SeoMetadataDto> CreateAsync(
            string entityType,
            Guid? entityId,
            string? routePath,
            string metaTitle,
            string? metaDescription,
            string? metaKeywords,
            string? canonicalUrl,
            string? ogTitle,
            string? ogDescription,
            string? ogImageUrl,
            string? robots,
            CancellationToken cancellationToken = default);

        Task<SeoMetadataDto> UpdateAsync(
            Guid id,
            string entityType,
            Guid? entityId,
            string? routePath,
            string metaTitle,
            string? metaDescription,
            string? metaKeywords,
            string? canonicalUrl,
            string? ogTitle,
            string? ogDescription,
            string? ogImageUrl,
            string? robots,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<SeoMetadataDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<SeoMetadataDto?> GetByEntityAsync(
            string entityType,
            Guid entityId,
            CancellationToken cancellationToken = default);

        Task<SeoMetadataDto?> GetByRoutePathAsync(
            string routePath,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<SeoMetadataDto>> GetPagedAsync(
            string? keyword,
            string? entityType,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
