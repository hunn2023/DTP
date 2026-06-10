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
    public class ConfirmOrderCommand : IRequest<Result>
    {
        public Guid OrderId { get; set; }

        public string? PaymentMethod { get; set; }

        public Guid? ChangedBy { get; set; }
    }

    public class ConfirmOrderCommandHandler : IRequestHandler<ConfirmOrderCommand, Result>
    {
        private readonly IOrderService _orderService;

        public ConfirmOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result> Handle(
            ConfirmOrderCommand request,
            CancellationToken cancellationToken)
        {
            return _orderService.ConfirmAsync(
                request.OrderId,
                request.PaymentMethod,
                request.ChangedBy,
                cancellationToken);
        }
    }
}
