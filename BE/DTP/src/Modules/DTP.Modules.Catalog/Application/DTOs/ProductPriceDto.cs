using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductPriceDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string? ProductName { get; set; }

        public Guid? ProductVariantId { get; set; }

        public string? ProductVariantName { get; set; }

        public string Currency { get; set; } = default!;

        public decimal OriginalPrice { get; set; }

        public decimal SalePrice { get; set; }

        public decimal CostPrice { get; set; }

        public decimal Profit => SalePrice - CostPrice;

        public decimal ProfitRate =>
            SalePrice <= 0
                ? 0
                : Math.Round((SalePrice - CostPrice) / SalePrice * 100, 2);

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
