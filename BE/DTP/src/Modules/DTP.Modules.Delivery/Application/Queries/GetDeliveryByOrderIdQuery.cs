using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Queries
{
    public record GetDeliveryByOrderIdQuery(Guid OrderId) : IRequest<Result<DeliveryDto>>;

    public class GetDeliveryByOrderIdQueryHandler
        : IRequestHandler<GetDeliveryByOrderIdQuery, Result<DeliveryDto>>
    {
        private readonly IDeliveryService _deliveryService;

        public GetDeliveryByOrderIdQueryHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task<Result<DeliveryDto>> Handle(
            GetDeliveryByOrderIdQuery request,
            CancellationToken cancellationToken)
        {
            return _deliveryService.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);
        }
    }
}
