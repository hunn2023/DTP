using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Clients
{
    public class VnptEpayOptions
    {
        public string PCodeRegister { get; set; } = default!;
        public string MerchantCode { get; set; } = default!;
        public string EncryptionKey { get; set; } = default!;
        public string RegisterVaUrl { get; set; } = default!;
        public string BankCode { get; set; } = default!;
        public string Condition { get; set; } = "03";
        public int VaExpireDays { get; set; } = 1;
        public string? EpayPublicKeyPem { get; set; }
    }
}
