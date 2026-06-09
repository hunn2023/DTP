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
    public record MarkDeliveryFailedCommand(
    Guid DeliveryId,
    string Error) : IRequest<Result>;

    public class MarkDeliveryFailedCommandHandler
        : IRequestHandler<MarkDeliveryFailedCommand, Result>
    {
        private readonly IDeliveryService _deliveryService;

        public MarkDeliveryFailedCommandHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task<Result> Handle(
            MarkDeliveryFailedCommand request,
            CancellationToken cancellationToken)
        {
            return _deliveryService.MarkFailedAsync(
                request.DeliveryId,
                request.Error,
                cancellationToken);
        }
    }
}
