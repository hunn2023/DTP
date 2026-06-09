using DTP.Modules.Delivery.Application.Abstractions.Services;
using DTP.Modules.Delivery.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Commands.DeliverOrder
{
    public class DeliverOrderCommand : IRequest<DeliverOrderResultDto>
    {
        public Guid OrderId { get; set; }

        public string? Note { get; set; }
    }

    public class DeliverOrderCommandHandler
       : IRequestHandler<DeliverOrderCommand, DeliverOrderResultDto>
    {
        private readonly IDeliveryService _deliveryService;

        public DeliverOrderCommandHandler(IDeliveryService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<DeliverOrderResultDto> Handle(
            DeliverOrderCommand request,
            CancellationToken cancellationToken)
        {
            return await _deliveryService.DeliverOrderAsync(new DeliverOrderDto
            {
                OrderId = request.OrderId,
                Note = request.Note
            }, cancellationToken);
        }
    }
}
