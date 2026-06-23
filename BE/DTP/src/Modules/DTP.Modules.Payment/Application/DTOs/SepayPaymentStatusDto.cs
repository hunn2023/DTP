using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public sealed class SepayPaymentStatusDto
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string PaymentCode { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;

        public string TransactionStatus { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public bool IsPaid { get; set; }

        public bool IsExpired { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
