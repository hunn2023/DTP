using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Commands.Orders
{
    public class CancelOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; set; }

        public string Reason { get; set; } = default!;

        public Guid? ChangedBy { get; set; }
    }

    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, Result>
    {
        private readonly IOrderService _orderService;

        public CancelOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result> Handle(
            CancelOrderCommand request,
            CancellationToken cancellationToken)
        {
            return _orderService.CancelAsync(
                request.OrderId,
                request.Reason,
                request.ChangedBy,
                cancellationToken);
        }
    }
}
