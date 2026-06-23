using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public sealed class SepayQrPaymentDto
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public string PaymentCode { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public string BankCode { get; set; } = string.Empty;

        public string AccountNumber { get; set; } = string.Empty;

        public string AccountName { get; set; } = string.Empty;

        public string QrImageUrl { get; set; } = string.Empty;

        public DateTime ExpiredAt { get; set; }
    }
}
