using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;
namespace DTP.Modules.Catalog.Application.Queries.Carriers
{
    public class GetPublicCarriersQuery
         : IRequest<Result<PagedResultDto<CarrierDto>>>
    {
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetPublicCarriersQueryHandler
       : IRequestHandler<GetPublicCarriersQuery, Result<PagedResultDto<CarrierDto>>>
    {
        private readonly ICarrierService _carrierService;

        public GetPublicCarriersQueryHandler(ICarrierService carrierService)
        {
            _carrierService = carrierService;
        }

        public async Task<Result<PagedResultDto<CarrierDto>>> Handle(
            GetPublicCarriersQuery request,
            CancellationToken cancellationToken)
        {
            return await _carrierService.GetPublicAsync(
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
