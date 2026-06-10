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
    public class MarkOrderPaidCommand : IRequest<Result>
    {
        public Guid OrderId { get; set; }

        public string PaymentTransactionId { get; set; } = default!;

        public Guid? ChangedBy { get; set; }
    }
    public class MarkOrderPaidCommandHandler : IRequestHandler<MarkOrderPaidCommand, Result>
    {
        private readonly IOrderService _orderService;

        public MarkOrderPaidCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public Task<Result> Handle(
            MarkOrderPaidCommand request,
            CancellationToken cancellationToken)
        {
            return _orderService.MarkPaidAsync(
                request.OrderId,
                request.PaymentTransactionId,
                request.ChangedBy,
                cancellationToken);
        }
    }
}
