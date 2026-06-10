using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class HomeCountryEsimDto
    {
        public Guid CountryId { get; set; }

        public string Code { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Slug { get; set; } = string.Empty;

        public string? FlagUrl { get; set; }

        public string? Region { get; set; }

        public decimal PriceFrom { get; set; }

        public string Currency { get; set; } = "VND";

        public int PackageCount { get; set; }

        public bool IsHot { get; set; }
    }
}
