using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.Commands.MarkOrderPaid;
using DTP.Modules.Ordering.Application.Commands.UpdateOrderStatus;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Modules.Payment.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class PaymentOrderingService : IPaymentOrderingService
    {
        private readonly IMediator _mediator;

        public PaymentOrderingService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task MarkOrderPaidAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new MarkOrderPaidCommand
            {
                OrderId = orderId,
                Note = note
            }, cancellationToken);
        }

        public async Task MarkOrderPaymentFailedAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new UpdateOrderStatusCommand
            {
                OrderId = orderId,
                Status = OrderStatus.PaymentFailed,
                Note = note
            }, cancellationToken);
        }
    }
}
