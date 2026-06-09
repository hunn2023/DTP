using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;
namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public class GetPublicEsimPackagesQuery
     : IRequest<Result<PagedResultDto<EsimPackageDto>>>
    {
        public Guid? CountryId { get; set; }

        public Guid? CarrierId { get; set; }

        public bool? IsUnlimited { get; set; }

        public int? ValidityDays { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetPublicEsimPackagesQueryHandler
      : IRequestHandler<GetPublicEsimPackagesQuery, Result<PagedResultDto<EsimPackageDto>>>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetPublicEsimPackagesQueryHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<Result<PagedResultDto<EsimPackageDto>>> Handle(
            GetPublicEsimPackagesQuery request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.GetPublicPagedAsync(
                request.CountryId,
                request.CarrierId,
                request.IsUnlimited,
                request.ValidityDays,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
