using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class FakeOrderingPaymentService : IOrderingPaymentService
    {
        public Task<CreatePaymentResultDto> CreatePaymentAsync(
            Guid orderId,
            string orderCode,
            decimal amount,
            string currencyCode,
            string customerEmail,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new CreatePaymentResultDto
            {
                PaymentTransactionCode = $"PAY-{DateTime.UtcNow:yyyyMMddHHmmss}",
                PaymentUrl = $"https://payment.local/{orderCode}",
                QrCodeUrl = $"https://qr.local/{orderCode}.png",
                ExpiredAt = DateTime.UtcNow.AddMinutes(15)
            });
        }
    }
}
