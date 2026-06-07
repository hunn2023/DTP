using DTP.Modules.Ordering.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Commands.CancelOrder
{
    public class CancelOrderCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public bool IsAdmin { get; set; }

        public string? Reason { get; set; }
    }


    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
    {
        private readonly IOrderService _orderService;

        public CancelOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<bool> Handle(
            CancelOrderCommand request,
            CancellationToken cancellationToken)
        {
            return await _orderService.CancelAsync(
                request.OrderId,
                request.UserId,
                request.IsAdmin,
                request.Reason,
                cancellationToken);
        }
    }
}
