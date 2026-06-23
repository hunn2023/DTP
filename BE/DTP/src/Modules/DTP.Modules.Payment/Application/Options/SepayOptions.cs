using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.Options
{
    public sealed class SepayOptions
    {
        public bool Enabled { get; set; } = true;

        public string AccountNumber { get; set; } = string.Empty;

        public string BankCode { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;

        public string PaymentCodePrefix { get; set; } = "DTP";

        public string WebhookSecret { get; set; } = string.Empty;

        public string Template { get; set; } = string.Empty;

        public string QrBaseUrl { get; set; } = string.Empty;

        public bool RequireWebhookSignature { get; set; } = true;
    }
}
