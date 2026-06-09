using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface ICarrierService
    {
        Task<Result<PagedResultDto<CarrierDto>>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<CarrierDto>>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<List<CarrierDto>>> GetActiveAsync(
        CancellationToken cancellationToken = default);

        Task<Result<CarrierDto?>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

        Task ClearCarrierCacheAsync(
        CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateAsync(
            string? code,
            string name,
            string slug,
            Guid countryId,
            string? logoUrl,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string? code,
            string name,
            string slug,
            Guid countryId,
            string? logoUrl,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
