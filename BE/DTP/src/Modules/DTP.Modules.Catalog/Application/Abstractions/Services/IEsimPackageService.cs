using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IEsimPackageService
    {
        Task<Result<PagedResultDto<EsimPackageDto>>> GetPublicPagedAsync(
           Guid? countryId,
           Guid? carrierId,
           bool? isUnlimited,
           int? validityDays,
           int pageIndex,
           int pageSize,
           CancellationToken cancellationToken = default);

        Task<Result<List<EsimPackageDto>>> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default);

        Task<Result<PagedResultDto<EsimPackageDto>>> GetPagedAsync(
            string? keyword,
            Guid? productVariantId,
            Guid? countryId,
            Guid? carrierId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Result<EsimPackageDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateAsync(
            CreateEsimPackageCommand command,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            UpdateEsimPackageCommand command,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
