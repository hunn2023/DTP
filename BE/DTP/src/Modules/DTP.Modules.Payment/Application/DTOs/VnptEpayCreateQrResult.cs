using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{
    public class VnptEpayCreateQrResult
    {
        public bool Success { get; set; }

        public string? ProviderTransactionCode { get; set; }

        public string? PaymentUrl { get; set; }

        public string? QrCodeUrl { get; set; }

        public string? QrContent { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public string? RawRequest { get; set; }

        public string? RawResponse { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
