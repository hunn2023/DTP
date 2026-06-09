using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Delivery.Application.Queries
{
    public record GetDeliveryByIdQuery(Guid Id) : IRequest<Result<DeliveryDto>>;

    public class GetDeliveryByIdQueryHandler
        : IRequestHandler<GetDeliveryByIdQuery, Result<DeliveryDto>>
    {
        private readonly IDeliveryService _deliveryService;

        public GetDeliveryByIdQueryHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task<Result<DeliveryDto>> Handle(
            GetDeliveryByIdQuery request,
            CancellationToken cancellationToken)
        {
            return _deliveryService.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
