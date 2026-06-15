using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProvisionProviderEsimProductRequest
    {
        public Guid ProviderId { get; set; }

        public string ProviderCode { get; set; } = default!;
        public string ProviderSku { get; set; } = default!;
        public string? ProviderProductId { get; set; }

        public string ProductName { get; set; } = default!;
        public string? ProductDescription { get; set; }

        public string VariantName { get; set; } = default!;
        public string? VariantSku { get; set; }

        public decimal Price { get; set; }
        public string CurrencyCode { get; set; } = "VND";

        public decimal? DataAmount { get; set; }
        public string? DataUnit { get; set; }
        public int ValidityDays { get; set; }
        public bool IsUnlimited { get; set; }

        public string? CoverageType { get; set; }
        public string? CoverageDescription { get; set; }

        public List<ProvisionCountryDto> Countries { get; set; } = new();
        public List<string> Operators { get; set; } = new();

        public bool IsActive { get; set; } = false;
    }
}
