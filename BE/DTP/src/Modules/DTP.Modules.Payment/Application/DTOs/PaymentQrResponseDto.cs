using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class PaymentQrResponseDto
    {
        public Guid PaymentTransactionId { get; set; }

        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "VND";

        public string Status { get; set; } = default!;

        public string? ProviderTransactionId { get; set; }

        public string? QrCode { get; set; }

        public string? QrImageUrl { get; set; }

        public string? PaymentUrl { get; set; }

        public string? BankCode { get; set; }

        public string? BankAccountNo { get; set; }

        public string? BankAccountName { get; set; }

        public string? TransferContent { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
