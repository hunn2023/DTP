using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class HomeEsimProductDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? LocationText { get; set; }

        public string? ThumbnailUrl { get; set; }

        public string? FlagUrl { get; set; }

        public decimal PriceFrom { get; set; }

        public string Currency { get; set; } = "VND";

        public bool IsHot { get; set; }

        public bool IsFeatured { get; set; }
    }

    public class HomeEsimCountryProductDto
    {
        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = default!;
        public string CountrySlug { get; set; } = default!;
        public string? FlagUrl { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string ProductSlug { get; set; } = default!;
        public string? LocationText { get; set; }
        public string? ThumbnailUrl { get; set; }

        public decimal PriceFrom { get; set; }
        public string Currency { get; set; } = "VND";

        public bool IsHot { get; set; }
        public bool IsFeatured { get; set; }
    }
}
