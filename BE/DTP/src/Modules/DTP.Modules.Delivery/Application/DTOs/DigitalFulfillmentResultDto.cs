using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class DigitalFulfillmentResultDto
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public List<DigitalFulfillmentItemDto> Items { get; set; } = new();
    }

    public class DigitalFulfillmentItemDto
    {
        public Guid? OrderItemId { get; set; }

        public string? QrCodeUrl { get; set; }

        public string? ActivationCode { get; set; }

        public string? SerialNumber { get; set; }

        public string? ProviderReference { get; set; }

        public string? RawData { get; set; }
    }
}
