using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderProvisionResultDto
    {
        public bool Success { get; set; }

        public Guid ProviderOrderId { get; set; }

        public string? ProviderOrderCode { get; set; }

        public string? ErrorCode { get; set; }

        public string? ErrorMessage { get; set; }

        public List<ProviderProvisionItemResultDto> Items { get; set; } = new();
    }

    public class ProviderProvisionItemResultDto
    {
        public Guid OrderItemId { get; set; }

        public string? Iccid { get; set; }

        public string? Msisdn { get; set; }

        public string? QrCodeUrl { get; set; }

        public string? QrCodeText { get; set; }

        public string? ActivationCode { get; set; }

        public string? Serial { get; set; }

        public string? PinCode { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
