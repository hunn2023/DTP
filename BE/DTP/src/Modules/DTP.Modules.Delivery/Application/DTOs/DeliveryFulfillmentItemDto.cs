using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public sealed class DeliveryFulfillmentItemDto
    {
        public Guid? OrderItemId { get; set; }

        public string? Sku { get; set; }

        public string SerialNumber { get; set; } = default!;

        public string? QrCodeUrl { get; set; }

        public string? ActivationCode { get; set; }

        public string? ProviderReference { get; set; }

        public string? RawData { get; set; }
    }
}
