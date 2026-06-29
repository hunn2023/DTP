using DTP.Modules.Delivery.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.WorkerService.Services
{
    public sealed class WorkerDeliveryRateLimitService : IDeliveryRateLimitService
    {
        public Task<bool> IsProcessBlockedAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task RegisterProcessAttemptAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsRetryBlockedAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task RegisterRetryAttemptAsync(
            Guid deliveryId,
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
