using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Services
{
    public interface IContentBannerService
    {
        Task<ContentBannerDto> CreateAsync(
            string title,
            string imageUrl,
            string? mobileImageUrl,
            string? linkUrl,
            string? description,
            ContentBannerPosition position,
            DateTime? startDate,
            DateTime? endDate,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<ContentBannerDto> UpdateAsync(
            Guid id,
            string title,
            string imageUrl,
            string? mobileImageUrl,
            string? linkUrl,
            string? description,
            ContentBannerPosition position,
            DateTime? startDate,
            DateTime? endDate,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<bool> DisableAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> EnableAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ContentBannerDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ContentBannerDto>> GetPagedAsync(
            string? keyword,
            ContentBannerPosition? position,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentBannerDto>> GetAvailableAsync(
            ContentBannerPosition? position,
            CancellationToken cancellationToken = default);
    }
}
