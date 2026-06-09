using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.Abstractions.Services
{
    public interface IContentFaqService
    {
        Task<ContentFaqDto> CreateAsync(
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<ContentFaqDto> UpdateAsync(
            Guid id,
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<bool> EnableAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> DisableAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<ContentFaqDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<ContentFaqDto>> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ContentFaqDto>> GetActiveAsync(
            string? categoryCode,
            CancellationToken cancellationToken = default);
    }
}
