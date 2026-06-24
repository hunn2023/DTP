using DTP.Modules.Ordering.Domain.Entities;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Repositories
{
    public interface IPaymentTransactionRepository : IRepositoryBase<PaymentTransaction>

    {
        Task<PaymentTransaction?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<PaymentTransaction?> GetPendingByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<PaymentTransaction?> GetByRequestIdAsync(string requestId, CancellationToken cancellationToken = default);

        Task<PaymentTransaction?> GetByProviderTransactionIdAsync(
            PaymentProvider provider,
            string providerTransactionId,
            CancellationToken cancellationToken = default);

        Task<bool> HasPaidPaymentByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

        Task<PaymentTransaction?> GetPendingVnptEpayByMapIdAndAmountAsync(
                string mapId,
                decimal amount,
                CancellationToken cancellationToken = default);


        Task<PaymentTransaction?> GetPendingSepayByTransferContentAsync(
            string transferContent,
            CancellationToken cancellationToken = default);


        Task<PaymentTransaction?> GetLatestByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);
    }
}
