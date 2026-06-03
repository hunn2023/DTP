using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IEsimPackageRepository : IRepositoryBase<EsimPackage>
    {
        Task<PagedResultDto<EsimPackageDto>> GetPublicPagedAsync(
             Guid? countryId,
             Guid? carrierId,
             bool? isUnlimited,
             int? validityDays,
             int pageIndex,
             int pageSize,
             CancellationToken cancellationToken = default);

        Task<EsimPackageDto?> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<EsimPackageDto>> GetPagedAsync(
            string? keyword,
            Guid? productVariantId,
            Guid? countryId,
            Guid? carrierId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<EsimPackageDto?> GetByIdDtoAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateAsync(
            CreateEsimPackageCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            UpdateEsimPackageCommand command,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
