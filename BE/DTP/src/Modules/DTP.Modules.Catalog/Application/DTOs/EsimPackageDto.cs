namespace DTP.Modules.Catalog.Application.DTOs
{
    public class EsimPackageDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;

        public Guid ProductVariantId { get; set; }
        public string ProductVariantName { get; set; } = default!;

        public Guid ProviderId { get; set; }
        public string ProviderName { get; set; } = default!;

        public Guid CountryId { get; set; }
        public string CountryName { get; set; } = default!;

        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string ProviderPackageCode { get; set; } = default!;

        public decimal? DataAmount { get; set; }
        public string DataUnit { get; set; } = default!;
        public int ValidityDays { get; set; }
        public bool IsUnlimited { get; set; }

        public string CoverageType { get; set; } = default!;
        public string? CoverageDescription { get; set; }
        public string ActivationPolicy { get; set; } = default!;
        public string? SpeedPolicy { get; set; }

        public bool HotspotSupported { get; set; }
        public bool PhoneNumberSupported { get; set; }
        public bool SmsSupported { get; set; }
        public bool KycRequired { get; set; }

        public string QrDeliveryType { get; set; } = default!;

        public int SortOrder { get; set; }
        public bool IsActive { get; set; }

        public List<EsimPackageCarrierDto> Carriers { get; set; } = new();
    }

    public class EsimPackageCarrierDto
    {
        public Guid CarrierId { get; set; }

        public string CarrierName { get; set; } = default!;
    }
}