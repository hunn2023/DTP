using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public sealed class SepayWebhookResultDto
    {
        public bool Success { get; set; }

        public bool Processed { get; set; }

        public string Message { get; set; } = string.Empty;

        public Guid? OrderId { get; set; }

        public Guid? PaymentTransactionId { get; set; }
    }
}
