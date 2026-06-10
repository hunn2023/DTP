using DTP.Modules.Ordering.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class CreateOrderItemRequest
    {
        public OrderItemType ItemType { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }

        public Guid? PhoneCardId { get; set; }

        public string ProductName { get; set; } = default!;

        public string? VariantName { get; set; }

        public string? Sku { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }

    public class CreateOrderItemCommandItem
    {
        public OrderItemType ItemType { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }

        public Guid? PhoneCardId { get; set; }

        public string ProductName { get; set; } = default!;

        public string? VariantName { get; set; }

        public string? Sku { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
