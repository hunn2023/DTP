using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.Queries.Carriers
{
    public class GetAdminCarriersQuery
        : IRequest<PagedResultDto<CarrierDto>>
    {
        public string? Keyword { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }


    public class GetAdminCarriersQueryHandler
        : IRequestHandler<GetAdminCarriersQuery, PagedResultDto<CarrierDto>>
    {
        private readonly ICarrierService _carrierService;

        public GetAdminCarriersQueryHandler(ICarrierService carrierService)
        {
            _carrierService = carrierService;
        }

        public async Task<PagedResultDto<CarrierDto>> Handle(
            GetAdminCarriersQuery request,
            CancellationToken cancellationToken)
        {
            return await _carrierService.GetPagedAsync(
                request.Keyword,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
