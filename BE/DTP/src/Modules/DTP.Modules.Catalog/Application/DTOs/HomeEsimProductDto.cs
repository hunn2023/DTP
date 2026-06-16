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
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;

        public string? LocationText { get; set; }
        public string? ThumbnailUrl { get; set; }

        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; }
        public string? CountrySlug { get; set; }
        public string? FlagUrl { get; set; }

        public bool IsHot { get; set; }
        public bool IsFeatured { get; set; }

        public decimal PriceFrom { get; set; }
        public string? Currency { get; set; }
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
