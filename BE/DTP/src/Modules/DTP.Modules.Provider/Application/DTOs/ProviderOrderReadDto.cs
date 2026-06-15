using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderOrderReadDto
    {
        public Guid OrderId { get; set; }

        public string OrderCode { get; set; } = default!;

        public Guid UserId { get; set; }

        public string CustomerEmail { get; set; } = default!;

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public decimal TotalAmount { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public List<ProviderOrderItemReadDto> Items { get; set; } = new();
    }

    public class ProviderOrderItemReadDto
    {
        public Guid OrderItemId { get; set; }

        public Guid ProductId { get; set; }

        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }

        public string Sku { get; set; } = default!;

        public string ProductName { get; set; } = default!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
