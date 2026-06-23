using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentSuccessHandler
    {
        Task HandlePaymentSuccessAsync(
            Guid orderId,
            Guid paymentTransactionId,
            string provider,
            string providerTransactionId,
            CancellationToken cancellationToken = default);
    }
}
