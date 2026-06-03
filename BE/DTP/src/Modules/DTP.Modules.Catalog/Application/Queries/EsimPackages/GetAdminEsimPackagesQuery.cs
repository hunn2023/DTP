using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public class GetAdminEsimPackagesQuery
     : IRequest<PagedResultDto<EsimPackageDto>>
    {
        public Guid? CountryId { get; set; }

        public Guid? CarrierId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public bool? IsActive { get; set; }

        public string? Keyword { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetAdminEsimPackagesQueryHandler
       : IRequestHandler<GetAdminEsimPackagesQuery, PagedResultDto<EsimPackageDto>>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetAdminEsimPackagesQueryHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<PagedResultDto<EsimPackageDto>> Handle(
            GetAdminEsimPackagesQuery request,
            CancellationToken cancellationToken)
        {
            return await _esimPackageService.GetPagedAsync(
                request.Keyword,
                request.ProductVariantId,
                request.CountryId,
                request.CarrierId,
                request.IsActive,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
