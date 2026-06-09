using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class CreatePaymentResultDto
    {
        public string PaymentTransactionCode { get; set; } = default!;
        public string? PaymentUrl { get; set; }
        public string? QrCodeUrl { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
}
