using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Domain.Enums;
using DTP.Modules.Payment.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace DTP.Modules.Payment.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : RepositoryBase<PaymentTransaction>,
         IPaymentTransactionRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentTransactionRepository(PaymentDbContext context) : base(context)
        {
            _context = context;
        }



        public Task<PaymentTransaction?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return _context.PaymentTransactions
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);
        }

        public Task<PaymentTransaction?> GetPendingByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return _context.PaymentTransactions
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(
                    x => x.OrderId == orderId &&
                         x.Status == PaymentStatus.Pending,
                    cancellationToken);
        }

        public Task<PaymentTransaction?> GetByRequestIdAsync(
            string requestId,
            CancellationToken cancellationToken = default)
        {
            return _context.PaymentTransactions
                .FirstOrDefaultAsync(x => x.RequestId == requestId, cancellationToken);
        }

        public Task<PaymentTransaction?> GetByProviderTransactionIdAsync(
            PaymentProvider provider,
            string providerTransactionId,
            CancellationToken cancellationToken = default)
        {
            return _context.PaymentTransactions
                .FirstOrDefaultAsync(
                    x => x.Provider == provider &&
                         x.ProviderTransactionId == providerTransactionId,
                    cancellationToken);
        }

        public Task<bool> HasPaidPaymentByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return _context.PaymentTransactions
                .AnyAsync(
                    x => x.OrderId == orderId &&
                         x.Status == PaymentStatus.Paid,
                    cancellationToken);
        }
    }
}
