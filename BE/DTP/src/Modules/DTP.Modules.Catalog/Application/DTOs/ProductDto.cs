using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public Guid CategoryId { get; set; }
        public Guid? CountryId { get; set; }
        public Guid? CarrierId { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        public List<ProductVariantDto> Variants { get; set; } = new();
        public List<ProductImageDto> Images { get; set; } = new();
        public List<ProductAttributeDto> Attributes { get; set; } = new();
    }
}
