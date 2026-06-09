using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{

    public class VnptEpayCallbackDto
    {
        public string? MerchantCode { get; set; }

        public string? RequestId { get; set; }

        public string? OrderCode { get; set; }

        public string? ProviderTransactionId { get; set; }

        public decimal Amount { get; set; }

        public string? Currency { get; set; }

        public string? Status { get; set; }

        public string? ResponseCode { get; set; }

        public string? Message { get; set; }

        public long Timestamp { get; set; }

        public string? Signature { get; set; }
    }
}
