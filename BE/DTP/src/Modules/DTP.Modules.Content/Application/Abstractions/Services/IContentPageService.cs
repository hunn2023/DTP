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
    public interface IContentPageService
    {
        Task<ContentPageDto> CreateAsync(
            string code,
            string title,
            string slug,
            string? summary,
            string content,
            ContentPageStatus status,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task<ContentPageDto> UpdateAsync(
            Guid id,
            string title,
            string slug,
            string? summary,
            string content,
            string? thumbnailUrl,
            ContentPageStatus status,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task<bool> HideAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ContentPageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ContentPageDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        Task<PagedResultDto<ContentPageDto>> GetPagedAsync(
            string? keyword,
            ContentPageStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentPageDto>> GetPublishedAsync(CancellationToken cancellationToken = default);
    }
}
