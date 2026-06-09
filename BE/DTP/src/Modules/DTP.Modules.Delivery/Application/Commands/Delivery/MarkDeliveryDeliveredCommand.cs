using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Commands.Delivery
{
    public record MarkDeliveryDeliveredCommand(
    Guid DeliveryId,
    string? Note) : IRequest<Result>;

    public class MarkDeliveryDeliveredCommandHandler
        : IRequestHandler<MarkDeliveryDeliveredCommand, Result>
    {
        private readonly IDeliveryService _deliveryService;

        public MarkDeliveryDeliveredCommandHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task<Result> Handle(
            MarkDeliveryDeliveredCommand request,
            CancellationToken cancellationToken)
        {
            return _deliveryService.MarkDeliveredAsync(
                request.DeliveryId,
                request.Note,
                cancellationToken);
        }
    }
}
