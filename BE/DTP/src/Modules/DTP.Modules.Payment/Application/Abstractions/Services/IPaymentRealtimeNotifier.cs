using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentRealtimeNotifier
    {
        Task NotifyPaymentPaidAsync(
            Guid orderId,
            Guid paymentId,
            string orderCode,
            CancellationToken cancellationToken = default);

        Task NotifyPaymentFailedAsync(
            Guid orderId,
            Guid paymentId,
            string orderCode,
            string reason,
            CancellationToken cancellationToken = default);
    }
}
