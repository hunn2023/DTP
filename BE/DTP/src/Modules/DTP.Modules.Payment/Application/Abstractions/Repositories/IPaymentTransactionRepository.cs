using DTP.Modules.Payment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Repositories
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransaction?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PaymentTransaction?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default);

        Task<PaymentTransaction?> GetByTransactionCodeAsync(
            string transactionCode,
            CancellationToken cancellationToken = default);

        Task<PaymentTransaction?> GetByOrderCodeAsync(
            string orderCode,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            PaymentTransaction transaction,
            CancellationToken cancellationToken = default);

        Task AddCallbackAsync(
            PaymentCallback callback,
            CancellationToken cancellationToken = default);

        void Update(PaymentTransaction transaction);

        IQueryable<PaymentTransaction> Query();
    }
}
