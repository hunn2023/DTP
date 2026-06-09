using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProviderService
    {
        Task<Result<List<ProviderDto>>> GetActiveAsync(CancellationToken cancellationToken = default);

        Task<Result<ProviderDto?>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

        Task ClearProviderCacheAsync(CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<ProviderDto>>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
