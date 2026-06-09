using DTP.Modules.Delivery.Application.DTOs;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.Abstractions.Services
{
    public interface IDeliveryService
    {
        Task<Result<Guid>> CreatePendingAsync(
            Guid orderId,
            string? ipAddress,
            CancellationToken cancellationToken = default);

        Task<Result> ProcessAsync(
            Guid deliveryId,
            CancellationToken cancellationToken = default); 

        Task<Result> MarkDeliveredAsync(
            Guid deliveryId,
            string? note,
            CancellationToken cancellationToken = default);

        Task<Result> MarkFailedAsync(
            Guid deliveryId,
            string error,
            CancellationToken cancellationToken = default);

        Task<Result<DeliveryDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result<DeliveryDto>> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);


        Task<Result> ResendEsimEmailAsync(
    Guid deliveryId,
    CancellationToken cancellationToken = default);
    }
}
