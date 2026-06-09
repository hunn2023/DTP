using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class VnptEpayCreateQrResponse
    {
        public bool IsSuccess { get; set; }

        public string? ResponseCode { get; set; }

        public string? Message { get; set; }

        public string? RequestId { get; set; }

        public string? ProviderTransactionId { get; set; }

        public string? ProviderPaymentCode { get; set; }

        public string? QrCode { get; set; }

        public string? QrImageUrl { get; set; }

        public string? PaymentUrl { get; set; }

        public string? BankCode { get; set; }

        public string? BankAccountNo { get; set; }

        public string? BankAccountName { get; set; }

        public string? TransferContent { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public string RawRequest { get; set; } = default!;

        public string RawResponse { get; set; } = default!;
    }
}
