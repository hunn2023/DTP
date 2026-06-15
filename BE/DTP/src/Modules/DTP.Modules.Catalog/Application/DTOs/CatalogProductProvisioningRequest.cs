using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public sealed class CatalogProductProvisioningRequest
    {
        public Guid ProviderId { get; set; }

        public string ProviderCode { get; set; } = default!;

        public string ProviderSku { get; set; } = default!;

        public string ProviderProductCode { get; set; } = default!;

        public string PackageName { get; set; } = default!;

        public string? Description { get; set; }

        public string CountryCode { get; set; } = default!;

        public string CountryName { get; set; } = default!;

        public string? RegionName { get; set; }

        public decimal? DataAmount { get; set; }

        public string DataUnit { get; set; } = "GB";

        public bool IsUnlimited { get; set; }

        public int ValidityDays { get; set; }

        public decimal Price { get; set; }

        public decimal CostPrice { get; set; }

        public string CurrencyCode { get; set; } = "VND";

        public string? OperatorName { get; set; }

        public string? CoverageType { get; set; }

        public string? CoverageDescription { get; set; }

        public string? ActivationPolicy { get; set; }

        public string? SpeedPolicy { get; set; }

        public bool HotspotSupported { get; set; }

        public bool PhoneNumberSupported { get; set; }

        public bool SmsSupported { get; set; }

        public bool KycRequired { get; set; }

        public List<CatalogProductProvisioningCoverageDto> Coverages { get; set; } = [];
    }

    public sealed class CatalogProductProvisioningCoverageDto
    {
        public string CountryCode { get; set; } = default!;

        public string CountryName { get; set; } = default!;

        public string? OperatorName { get; set; }

        public string? Network { get; set; }

        public string? Speed { get; set; }
    }
}
