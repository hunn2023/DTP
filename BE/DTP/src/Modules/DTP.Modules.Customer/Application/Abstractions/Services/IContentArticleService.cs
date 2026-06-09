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
    public interface IContentArticleService
    {
        Task<ContentArticleDto> CreateAsync(
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
            CancellationToken cancellationToken = default);

        Task<ContentArticleDto> UpdateAsync(
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
            CancellationToken cancellationToken = default);

        Task<bool> PublishAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> HideAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> MarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> UnmarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ContentArticleDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ContentArticleDto?> GetBySlugAsync(
            string slug,
            bool increaseView,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ContentArticleListItemDto>> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            ContentArticleStatus? status,
            bool? isFeatured,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ContentArticleListItemDto>> GetPublicPagedAsync(
            string? keyword,
            string? categoryCode,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentArticleListItemDto>> GetFeaturedAsync(
            int take,
            CancellationToken cancellationToken = default);
    }
}
