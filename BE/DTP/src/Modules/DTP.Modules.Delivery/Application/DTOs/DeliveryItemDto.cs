using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Application.DTOs
{
    public class DeliveryItemDto
    {
        public Guid Id { get; set; }

        public Guid? OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public string ProductName { get; set; } = default!;

        public string? Sku { get; set; }

        public int Quantity { get; set; }

        public string? QrCodeUrl { get; set; }

        public string? ActivationCode { get; set; }

        public string? SerialNumber { get; set; }

        public string? ProviderReference { get; set; }

        public bool IsDelivered { get; set; }

        public DateTime? DeliveredAt { get; set; }
    }
}
