using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.EsimPackages
{
    public class GetPublicEsimPackagesQuery
     : IRequest<PagedResultDto<EsimPackageDto>>
    {
        public Guid? CountryId { get; set; }

        public Guid? CarrierId { get; set; }

        public bool? IsUnlimited { get; set; }

        public int? ValidityDays { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetPublicEsimPackagesQueryHandler
      : IRequestHandler<GetPublicEsimPackagesQuery, PagedResultDto<EsimPackageDto>>
    {
        private readonly IEsimPackageService _esimPackageService;

        public GetPublicEsimPackagesQueryHandler(
            IEsimPackageService esimPackageService)
        {
            _esimPackageService = esimPackageService;
        }

        public async Task<PagedResultDto<EsimPackageDto>> Handle(
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
