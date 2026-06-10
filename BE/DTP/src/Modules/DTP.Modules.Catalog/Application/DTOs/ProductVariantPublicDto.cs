using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductVariantPublicDto
    {
        public Guid ProductId { get; set; }
        public string? ProductCode { get; set; }
        public string ProductName { get; set; } = default!;
        public string ProductSlug { get; set; } = default!;

        public Guid ProductVariantId { get; set; }
        public string? Sku { get; set; }
        public string VariantName { get; set; } = default!;
        public string? VariantShortName { get; set; }
        public string? VariantDescription { get; set; }

        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }

        public string? ShortDescription { get; set; }
        public string? LocationText { get; set; }
        public string? ThumbnailUrl { get; set; }

        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string Currency { get; set; } = "VND";

        public bool IsFeatured { get; set; }
        public bool IsHot { get; set; }
        public int SoldCount { get; set; }

        public int ProductSortOrder { get; set; }
        public int VariantSortOrder { get; set; }

        public List<ProductVariantFeaturePublicDto> Features { get; set; } = new();
    }


    public class ProductVariantFeaturePublicDto
    {
        //public Guid Id { get; set; }

        public Guid ProductVariantId { get; set; }

        public string Text { get; set; } = string.Empty;

        public string? Icon { get; set; }

        public int SortOrder { get; set; }
    }
}
