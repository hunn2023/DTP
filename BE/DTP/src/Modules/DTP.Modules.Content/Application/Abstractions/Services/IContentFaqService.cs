using DTP.Modules.Content.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Content.Application.Abstractions.Services
{
    public interface IContentFaqService
    {
        Task<Result<ContentFaqDto>> CreateAsync(
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result<ContentFaqDto>> UpdateAsync(
            Guid id,
            string question,
            string answer,
            string? categoryCode,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result> EnableAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result> DisableAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<ContentFaqDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<ContentFaqDto>>> GetPagedAsync(
            string? keyword,
            string? categoryCode,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<IReadOnlyList<ContentFaqDto>>> GetActiveAsync(
            string? categoryCode,
            CancellationToken cancellationToken = default);
    }
}
