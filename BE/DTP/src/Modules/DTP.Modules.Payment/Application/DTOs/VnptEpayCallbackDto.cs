using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Application.DTOs
{

    public class VnptEpayCallbackDto
    {
        public string? OrderCode { get; set; }

        public string? TransactionCode { get; set; }

        public string? ProviderTransactionCode { get; set; }

        public decimal Amount { get; set; }

        public string? Status { get; set; }

        public string? Message { get; set; }

        public string? Signature { get; set; }

        public string RawBody { get; set; } = default!;
    }
}
