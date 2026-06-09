using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductVariantFeatureDto
    {
        public Guid Id { get; set; }

        public Guid ProductVariantId { get; set; }

        public string Text { get; set; } = default!;

        public string? Icon { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
