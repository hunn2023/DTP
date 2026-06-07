using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Queries
{
    public class GetDeliveryByOrderIdQuery : IRequest<List<DigitalDeliveryDto>>
    {
        public Guid OrderId { get; set; }
    }

    public class GetDeliveryByOrderIdQueryHandler
        : IRequestHandler<GetDeliveryByOrderIdQuery, List<DigitalDeliveryDto>>
    {
        private readonly IDeliveryService _deliveryService;

        public GetDeliveryByOrderIdQueryHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<List<DigitalDeliveryDto>> Handle(
            GetDeliveryByOrderIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _deliveryService.GetDeliveriesByOrderIdAsync(
                request.OrderId,
                cancellationToken);
        }
    }
}
