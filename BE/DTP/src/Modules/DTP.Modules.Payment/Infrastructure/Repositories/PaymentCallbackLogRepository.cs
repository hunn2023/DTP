using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using DTP.Modules.Payment.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Payment.Infrastructure.Repositories
{
    public class PaymentCallbackLogRepository : RepositoryBase<PaymentCallbackLog>,
          IPaymentCallbackLogRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentCallbackLogRepository(PaymentDbContext context) : base(context)
        {
            _context = context;
        }


        public Task<bool> ExistsProcessedAsync(
            string? requestId,
            string? providerTransactionId,
            CancellationToken cancellationToken = default)
        {
            return _context.PaymentCallbackLogs.AnyAsync(
                x =>
                    x.Status == PaymentCallbackStatus.Processed &&
                    (
                        (!string.IsNullOrWhiteSpace(requestId) && x.RequestId == requestId) ||
                        (!string.IsNullOrWhiteSpace(providerTransactionId) && x.ProviderTransactionId == providerTransactionId)
                    ),
                cancellationToken);
        }


        public async Task<bool> ExistsProcessedByProviderTransactionIdAsync(
                 Guid paymentProviderId,
                string providerTransactionId,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(providerTransactionId))
                return false;

            providerTransactionId = providerTransactionId.Trim();

            return await _context.PaymentCallbackLogs
                .AsNoTracking()
                .AnyAsync(x =>
                    x.PaymentProviderId == paymentProviderId &&
                    x.ProviderTransactionId == providerTransactionId &&
                    x.Status == PaymentCallbackStatus.Processed,
                    cancellationToken);
        }
    }
}
