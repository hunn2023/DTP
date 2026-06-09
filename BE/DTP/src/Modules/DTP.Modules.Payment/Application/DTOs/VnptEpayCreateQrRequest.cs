using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class VnptEpayCreateQrRequest
    {
        public string MerchantCode { get; set; } = default!;

        public string RequestId { get; set; } = default!;

        public string OrderCode { get; set; } = default!;

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "VND";

        public string Description { get; set; } = default!;

        public string ReturnUrl { get; set; } = default!;

        public string CallbackUrl { get; set; } = default!;

        public long Timestamp { get; set; }

        public string Signature { get; set; } = default!;
    }
}
