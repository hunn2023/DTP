using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Delivery
{
    public class DeliveryOrderInfoDto
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid? CustomerId { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }

        public SharedDeliveryType DeliveryType { get; set; }

        public bool IsPaid { get; set; }

        public List<DeliveryOrderItemDto> Items { get; set; } = new();
    }

    public class DeliveryOrderItemDto
    {
        public Guid? OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public string ProductName { get; set; } = default!;

        public string? Sku { get; set; }

        public int Quantity { get; set; }
    }
}
