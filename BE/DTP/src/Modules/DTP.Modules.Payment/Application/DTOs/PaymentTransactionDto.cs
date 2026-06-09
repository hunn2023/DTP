using DTP.Modules.Payment.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class PaymentTransactionDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public string TransactionCode { get; set; } = default!;

        public string? ProviderTransactionCode { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public PaymentTransactionStatus Status { get; set; }

        public string StatusName { get; set; } = default!;

        public string? PaymentUrl { get; set; }

        public string? QrCodeUrl { get; set; }

        public string? QrContent { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
