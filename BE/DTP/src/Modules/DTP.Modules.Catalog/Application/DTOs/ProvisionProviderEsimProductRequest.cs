using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public sealed class ProvisionProviderEsimProductRequest
    {
        public Guid ProviderId { get; init; }

        public string ProviderCode { get; init; } = null!;

        public string ProviderSku { get; init; } = null!;

        public string ProviderProductId { get; init; } = null!;

        // Dùng để gom nhiều ProviderSku vào cùng Product
        public string ProductFamilyCode { get; init; } = null!;

        public string? Slug { get; init; }

        public string ProductName { get; init; } = null!;

        public string? ProductDescription { get; init; }

        public string VariantName { get; init; } = null!;

        // Đây phải là SKU nội bộ DTP, không phải ProviderSku
        public string VariantSku { get; init; } = null!;

        public decimal Price { get; init; }

        public string CurrencyCode { get; init; } = "VND";

        public decimal DataAmount { get; init; }

        public string DataUnit { get; init; } = "MB";

        public int ValidityDays { get; init; }

        public bool IsUnlimited { get; init; }

        public string? CoverageType { get; init; }

        public string? CoverageDescription { get; init; }

        public List<ProvisionCountryDto> Countries { get; init; } = [];

        public List<string> Operators { get; init; } = [];

        public bool IsActive { get; init; }

        public int DataType { get; init; }
    }
}
