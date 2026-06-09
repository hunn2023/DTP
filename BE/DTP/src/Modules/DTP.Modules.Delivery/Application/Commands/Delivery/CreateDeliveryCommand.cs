using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Delivery.Application.Commands.DeliverOrder
{
    public record CreateDeliveryCommand(
    Guid OrderId,
    string? IpAddress) : IRequest<Result<Guid>>;

    public class CreateDeliveryCommandHandler
        : IRequestHandler<CreateDeliveryCommand, Result<Guid>>
    {
        private readonly IDeliveryService _deliveryService;

        public CreateDeliveryCommandHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task<Result<Guid>> Handle(
            CreateDeliveryCommand request,
            CancellationToken cancellationToken)
        {
            return _deliveryService.CreatePendingAsync(
                request.OrderId,
                request.IpAddress,
                cancellationToken);
        }
    }
}
