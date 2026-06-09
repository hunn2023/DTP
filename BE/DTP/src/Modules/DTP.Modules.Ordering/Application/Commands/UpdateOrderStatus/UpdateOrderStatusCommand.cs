using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : IRequest<Result>
    {
        public Guid OrderId { get; set; }

        public OrderStatus Status { get; set; }

        public string? Note { get; set; }

        public Guid? UpdatedBy { get; set; }
    }

    public class UpdateOrderStatusCommandHandler
       : IRequestHandler<UpdateOrderStatusCommand, Result>
    {
        private readonly IOrderService _orderService;

        public UpdateOrderStatusCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<Result> Handle(
            UpdateOrderStatusCommand request,
            CancellationToken cancellationToken)
        {
            return await _orderService.UpdateStatusAsync(
                request.OrderId,
                request.Status,
                request.Note,
                cancellationToken);
        }
    }
}
