using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        public string ProductName { get; set; } = default!;
        public string? VariantName { get; set; }
        public string? Sku { get; set; }
        public string? ThumbnailUrl { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public string CurrencyCode { get; set; } = "VND";
    }
}
