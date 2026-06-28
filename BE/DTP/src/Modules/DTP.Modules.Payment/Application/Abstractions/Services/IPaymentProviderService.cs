using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentProviderService
    {
        Task<IReadOnlyList<PaymentProviderPublicDto>> GetPublicActiveAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentProviderAdminDto>> GetAdminListAsync(
            CancellationToken cancellationToken = default);

        Task SetActiveAsync(
            Guid id,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task SetDefaultAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task UpdateLimitsAsync(
            Guid id,
            decimal? minAmount,
            decimal? maxAmount,
            CancellationToken cancellationToken = default);

        Task UpdateSortOrderAsync(
            Guid id,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task<PaymentProvider> GetActiveProviderForCheckoutAsync(
            string? providerCode,
            decimal amount,
            string currency,
            CancellationToken cancellationToken = default);


        Task<PaymentProviderValidationResult> ValidateForCreateQrAsync(
          string? paymentProviderCode,
          decimal amount,
          string currency,
          CancellationToken cancellationToken = default);
    }
}
