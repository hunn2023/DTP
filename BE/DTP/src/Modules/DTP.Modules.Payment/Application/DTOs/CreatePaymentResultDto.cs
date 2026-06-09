using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class CreatePaymentResultDto
    {
        public Guid PaymentTransactionId { get; set; }

        public string PaymentTransactionCode { get; set; } = default!;

        public string? ProviderTransactionCode { get; set; }

        public string? PaymentUrl { get; set; }

        public string? QrCodeUrl { get; set; }

        public string? QrContent { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
