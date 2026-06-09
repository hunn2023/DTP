using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class VnptEpayCreateQrRequest
    {
        public string TransactionCode { get; set; } = default!;

        public string OrderCode { get; set; } = default!;

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public string CustomerEmail { get; set; } = default!;

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public string? ReturnUrl { get; set; }

        public string? CallbackUrl { get; set; }
    }
}
