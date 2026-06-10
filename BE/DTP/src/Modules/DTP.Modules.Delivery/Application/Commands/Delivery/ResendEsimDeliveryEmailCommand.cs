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
    public record ResendEsimDeliveryEmailCommand(Guid DeliveryId) : IRequest<Result>;

    public class ResendEsimDeliveryEmailCommandHandler
        : IRequestHandler<ResendEsimDeliveryEmailCommand, Result>
    {
        private readonly IDeliveryService _deliveryService;

        public ResendEsimDeliveryEmailCommandHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task<Result> Handle(
            ResendEsimDeliveryEmailCommand request,
            CancellationToken cancellationToken)
        {
            return _deliveryService.ResendEsimEmailAsync(
                request.DeliveryId,
                cancellationToken);
        }
    }
}
