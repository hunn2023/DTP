using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentRateLimitService
    {
        Task<bool> IsCreateQrBlockedAsync(
            Guid orderId,
            Guid? customerId,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterCreateQrAttemptAsync(
            Guid orderId,
            Guid? customerId,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task<bool> IsCallbackBlockedAsync(
            string? transactionCode,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterCallbackAttemptAsync(
            string? transactionCode,
            string? ipAddress,
            CancellationToken cancellationToken = default);
    }
}
