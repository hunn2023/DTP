using DTP.Modules.Ordering.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.Abstractions.Services
{
    public interface IOrderingPaymentService
    {
        Task<CreatePaymentResultDto> CreatePaymentAsync(
            Guid orderId,
            string orderCode,
            decimal amount,
            string currencyCode,
            string customerEmail,
            CancellationToken cancellationToken = default);
    }
}
