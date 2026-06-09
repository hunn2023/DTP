using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Infrastructure.Clients
{
    public class VnptEpayOptions
    {
        public string BaseUrl { get; set; } = default!;

        public string CreateQrEndpoint { get; set; } = "/api/v1/payments/qr/create";

        public string MerchantCode { get; set; } = default!;

        public string SecretKey { get; set; } = default!;

        public string ReturnUrl { get; set; } = default!;

        public string CallbackUrl { get; set; } = default!;

        public int QrExpiredMinutes { get; set; } = 15;

        public int TimeoutSeconds { get; set; } = 30;
    }
}
