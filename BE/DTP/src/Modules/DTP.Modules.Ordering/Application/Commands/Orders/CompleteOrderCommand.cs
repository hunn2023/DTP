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
    public class CompleteOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; set; }

        public Guid? ChangedBy { get; set; }
    }

    public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, Result>
    {
        private readonly IOrderService _orderService;

        public CompleteOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result> Handle(
            CompleteOrderCommand request,
            CancellationToken cancellationToken)
        {
            return _orderService.CompleteAsync(
                request.OrderId,
                request.ChangedBy,
                cancellationToken);
        }
    }
}
