using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Application.DTOs
{
    public class OrderingProductSnapshotDto
    {
        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        public Guid? EsimPackageId { get; set; }
        public Guid? PhoneCardId { get; set; }

        public string ProductCode { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public string? ProductSlug { get; set; }
        public string? VariantName { get; set; }
        public string? Sku { get; set; }
        public string? ThumbnailUrl { get; set; }

        public decimal UnitPrice { get; set; }
        public string CurrencyCode { get; set; } = "VND";

        public bool IsActive { get; set; }
    }
}
