using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IEsimPackageService
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

        Task<EsimPackageDto?> GetByIdAsync(
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
