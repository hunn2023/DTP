using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public sealed class PaymentOrderStatusDto
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public Guid? PaymentTransactionId { get; set; }

        public string Status { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "VND";

        public bool IsPaid { get; set; }

        public bool IsExpired { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}
