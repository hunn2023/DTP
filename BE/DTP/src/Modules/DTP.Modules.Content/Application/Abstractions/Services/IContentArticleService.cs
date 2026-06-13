using DTP.Modules.Content.Application.DTOs;
using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using Microsoft.AspNetCore.Http;

namespace DTP.Modules.Content.Application.Abstractions.Services
{
    public interface IContentArticleService
    {
        Task<Result<ContentArticleDto>> CreateAsync(
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

        Task<Result<ContentArticleDto>> UpdateAsync(
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

        Task<Result> PublishAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result> HideAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result> MarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result> UnmarkAsFeaturedAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<ContentArticleDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<ContentArticleDto?>> GetBySlugAsync(
            string slug,
            bool increaseView,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<ContentArticleListItemDto>>> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            ContentArticleStatus? status,
            bool? isFeatured,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<ContentArticleListItemDto>>> GetPublicPagedAsync(
            string? keyword,
            string? categoryCode,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<ContentArticleListItemDto>>> GetFeaturedAsync(
            int take,
            CancellationToken cancellationToken = default);


        Task<Result<ContentArticleDto>> UploadThumbnailAsync(
            Guid articleId,
            IFormFile file,
            CancellationToken cancellationToken = default);
    }
}
