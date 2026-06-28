using DTP.Modules.Payment.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Repositories
{
    public interface IPaymentProviderRepository : IRepositoryBase<PaymentProvider>
    {
        Task<IReadOnlyList<PaymentProvider>> GetPublicActiveAsync(
       CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentProvider>> GetAdminListAsync(
            CancellationToken cancellationToken = default);

        Task<PaymentProvider?> GetActiveByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<PaymentProvider?> GetDefaultActiveAsync(
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<PaymentProvider>> GetAllTrackingAsync(
            CancellationToken cancellationToken = default);

        Task<PaymentProvider?> GetAlternativeActiveProviderAsync(
            Guid excludeId,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveDefaultAsync(
            CancellationToken cancellationToken = default);

        Task<PaymentProvider?> GetDefaultAvailableAsync(
           string currency,
           decimal amount,
           CancellationToken cancellationToken = default);
    }
}
