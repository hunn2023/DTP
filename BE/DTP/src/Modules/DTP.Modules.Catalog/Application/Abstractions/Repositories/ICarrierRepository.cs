using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{

    public interface ICarrierRepository : IRepositoryBase<Carrier>
    {
        Task<PagedResultDto<CarrierDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<CarrierDto>> GetPublicPagedAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<List<Carrier>> GetListAsync(
            string? keyword,
            Guid? countryId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            Guid countryId,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsBySlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);
    }
}
