using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface IPaymentOrderingService
    {
        Task MarkOrderPaidAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default);

        Task MarkOrderPaymentFailedAsync(
            Guid orderId,
            string? note,
            CancellationToken cancellationToken = default);
    }
}
