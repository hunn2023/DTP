using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Abstractions.Services
{
    public interface IDeliveryRateLimitService
    {
        Task<bool> IsProcessBlockedAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterProcessAttemptAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task<bool> IsRetryBlockedAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task RegisterRetryAttemptAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default);
    }
}
