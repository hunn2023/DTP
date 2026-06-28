using DTP.Modules.Payment.Application.DTOs;
using DTP.Shared.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Abstractions.Services
{
    public interface ISepayPaymentService
    {
        Task<Result<PaymentQrResponseDto>> CreateQrAsync(
            Guid orderId,
             string paymentProviderCode,
            string ipAddress,
            CancellationToken cancellationToken = default);


        Task<Result<bool>> HandleSepayWebhookAsync(
            SepayWebhookDto callback,
            string rawBody,
            string? signature,
            string? ipAddress,
            CancellationToken cancellationToken = default);
    }
}
