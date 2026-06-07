using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Modules.Delivery.Domain.Enums;
using DTP.Shared.Application.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Queries
{
    public class GetEsimProfilesQuery : IRequest<PagedResultDto<EsimProfileDto>>
    {
        public Guid? ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public EsimProfileStatus? Status { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetEsimProfilesQueryHandler
       : IRequestHandler<GetEsimProfilesQuery, PagedResultDto<EsimProfileDto>>
    {
        private readonly IDeliveryService _deliveryService;

        public GetEsimProfilesQueryHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<PagedResultDto<EsimProfileDto>> Handle(
            GetEsimProfilesQuery request,
            CancellationToken cancellationToken)
        {
            return await _deliveryService.GetEsimProfilesAsync(
                request.ProductId,
                request.ProductVariantId,
                request.Status,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
