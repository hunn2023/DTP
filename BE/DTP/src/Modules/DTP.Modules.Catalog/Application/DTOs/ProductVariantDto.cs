using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string? Sku { get; set; }

        public string Name { get; set; } = default!;

        public string? ShortName { get; set; }

        public string? Description { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
