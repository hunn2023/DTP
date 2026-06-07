using DTP.Modules.Payment.Application.Abstractions.Repositories;
using DTP.Modules.Payment.Domain.Entities;
using DTP.Modules.Payment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace DTP.Modules.Payment.Infrastructure.Repositories
{
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly PaymentDbContext _context;

        public PaymentTransactionRepository(PaymentDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentTransaction?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<PaymentTransaction?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(x => x.OrderId == orderId && !x.IsDeleted, cancellationToken);
        }

        public async Task<PaymentTransaction?> GetByTransactionCodeAsync(
            string transactionCode,
            CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(
                    x => x.TransactionCode == transactionCode && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<PaymentTransaction?> GetByOrderCodeAsync(
            string orderCode,
            CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(
                    x => x.OrderCode == orderCode && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task AddAsync(
            PaymentTransaction transaction,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentTransactions.AddAsync(transaction, cancellationToken);
        }

        public async Task AddCallbackAsync(
            PaymentCallback callback,
            CancellationToken cancellationToken = default)
        {
            await _context.PaymentCallbacks.AddAsync(callback, cancellationToken);
        }

        public void Update(PaymentTransaction transaction)
        {
            _context.PaymentTransactions.Update(transaction);
        }

        public IQueryable<PaymentTransaction> Query()
        {
            return _context.PaymentTransactions.AsQueryable();
        }
    }
}
