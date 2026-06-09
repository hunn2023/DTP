using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class FakeVnptEpayClient : IVnptEpayClient
    {
        public Task<VnptEpayCreateQrResult> CreateQrPaymentAsync(
            VnptEpayCreateQrRequest request,
            CancellationToken cancellationToken = default)
        {
            var rawRequest = JsonSerializer.Serialize(request);

            var result = new VnptEpayCreateQrResult
            {
                Success = true,
                ProviderTransactionCode = $"VNPT-{request.TransactionCode}",
                PaymentUrl = $"https://sandbox-payment.vnpt-epay.local/pay/{request.TransactionCode}",
                QrCodeUrl = $"https://sandbox-payment.vnpt-epay.local/qr/{request.TransactionCode}.png",
                QrContent = $"VNPT_EPAY|{request.OrderCode}|{request.TransactionCode}|{request.Amount}",
                ExpiredAt = DateTime.UtcNow.AddMinutes(15),
                RawRequest = rawRequest,
                RawResponse = JsonSerializer.Serialize(new
                {
                    code = "00",
                    message = "Success",
                    transactionCode = request.TransactionCode
                })
            };

            return Task.FromResult(result);
        }

        public bool VerifyCallbackSignature(VnptEpayCallbackDto request)
        {
            // TODO: Khi có tài liệu VNPT ePay thật thì verify HMAC/signature tại đây.
            // Giai đoạn fake tạm thời return true.
            return true;
        }
    }
}
