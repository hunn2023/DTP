using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public sealed class PaymentRealtimeNotifier : IPaymentRealtimeNotifier
    {
        private readonly IHubContext<PaymentHub> _hubContext;

        public PaymentRealtimeNotifier(IHubContext<PaymentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyPaymentPaidAsync(
            Guid orderId,
            Guid paymentId,
            string orderCode,
            CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group(PaymentHub.GetOrderPaymentGroup(orderId.ToString()))
                .SendAsync(
                    "PaymentPaid",
                    new
                    {
                        orderId,
                        paymentId,
                        orderCode,
                        status = "Paid",
                        paidAt = DateTime.UtcNow
                    },
                    cancellationToken);
        }

        public async Task NotifyPaymentFailedAsync(
            Guid orderId,
            Guid paymentId,
            string orderCode,
            string reason,
            CancellationToken cancellationToken = default)
        {
            await _hubContext.Clients
                .Group(PaymentHub.GetOrderPaymentGroup(orderId.ToString()))
                .SendAsync(
                    "PaymentFailed",
                    new
                    {
                        orderId,
                        paymentId,
                        orderCode,
                        status = "Failed",
                        reason
                    },
                    cancellationToken);
        }
    }
}
