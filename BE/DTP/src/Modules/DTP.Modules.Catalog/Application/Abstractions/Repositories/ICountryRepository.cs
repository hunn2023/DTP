using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface ICountryRepository
      : IRepositoryBase<Country>
    {

        Task<List<Country>> GetActiveListAsync(
            CancellationToken cancellationToken = default);

        Task<Country?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<bool> ExistsByCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsBySlugAsync(
            string slug,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<CountryDto>> GetPublicPagedAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<CountryDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);


        Task<PagedResultDto<HomeCountryEsimDto>> GetHomeCountriesAsync(
            string? region,
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
