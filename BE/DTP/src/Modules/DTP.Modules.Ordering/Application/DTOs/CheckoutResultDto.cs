using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class CheckoutResultDto
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; } = default!;

        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "VND";

        public string PaymentTransactionCode { get; set; } = default!;
        public string? PaymentUrl { get; set; }
        public string? QrCodeUrl { get; set; }
        public DateTime? PaymentExpiredAt { get; set; }
    }
}
