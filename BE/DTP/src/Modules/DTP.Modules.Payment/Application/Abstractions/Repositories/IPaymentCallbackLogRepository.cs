using DTP.Modules.Payment.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Repositories
{
    public interface IPaymentCallbackLogRepository : IRepositoryBase<PaymentCallbackLog>
    {
        Task<bool> ExistsProcessedAsync(
            string? requestId,
            string? providerTransactionId,
            CancellationToken cancellationToken = default);
    }
}
