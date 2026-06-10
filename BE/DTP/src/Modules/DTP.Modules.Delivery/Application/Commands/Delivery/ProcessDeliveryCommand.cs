using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Commands.Delivery
{
    public record ProcessDeliveryCommand(Guid DeliveryId,string? IpAddress) : IRequest<Result>;

    public class ProcessDeliveryCommandHandler
        : IRequestHandler<ProcessDeliveryCommand, Result>
    {
        private readonly IDeliveryService _deliveryService;

        public ProcessDeliveryCommandHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public Task<Result> Handle(
            ProcessDeliveryCommand request,
            CancellationToken cancellationToken)
        {
            return _deliveryService.ProcessAsync(
                request.DeliveryId,
                request.IpAddress,
                cancellationToken);
        }
    }
}
