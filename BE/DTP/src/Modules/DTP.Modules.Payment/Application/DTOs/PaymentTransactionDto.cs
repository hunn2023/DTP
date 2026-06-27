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

        public Guid? CustomerId { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "VND";

        public string Provider { get; set; } = default!;

        public string Method { get; set; } = default!;

        public string Status { get; set; } = default!;

        public string RequestId { get; set; } = default!;

        public Guid PaymentProviderId { get; set; }
        public string? ProviderTransactionId { get; set; }

        public string? QrCode { get; set; }

        public string? QrImageUrl { get; set; }

        public string? PaymentUrl { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
