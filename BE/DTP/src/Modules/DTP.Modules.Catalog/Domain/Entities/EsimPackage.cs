using DTP.Shared.Domain;


namespace DTP.Modules.Catalog.Domain.Entities
{
    public class EsimPackage : EntityBase
    {
        private readonly List<EsimPackageCarrier> _carriers = new();

        private EsimPackage()
        {
        }

        public EsimPackage(
            Guid productId,
            Guid productVariantId,
            Guid providerId,
            Guid countryId,
            string name,
            string slug,
            string providerPackageCode,
            decimal? dataAmount,
            string dataUnit,
            int validityDays,
            bool isUnlimited,
            string coverageType,
            string? coverageDescription,
            string activationPolicy,
            string? speedPolicy,
            bool hotspotSupported,
            bool phoneNumberSupported,
            bool smsSupported,
            bool kycRequired,
            string qrDeliveryType,
            int sortOrder,
            bool isActive)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProviderId = providerId;
            CountryId = countryId;
            Name = name;
            Slug = slug;
            ProviderPackageCode = providerPackageCode;
            DataAmount = dataAmount;
            DataUnit = dataUnit;
            ValidityDays = validityDays;
            IsUnlimited = isUnlimited;
            CoverageType = coverageType;
            CoverageDescription = coverageDescription;
            ActivationPolicy = activationPolicy;
            SpeedPolicy = speedPolicy;
            HotspotSupported = hotspotSupported;
            PhoneNumberSupported = phoneNumberSupported;
            SmsSupported = smsSupported;
            KycRequired = kycRequired;
            QrDeliveryType = qrDeliveryType;
            SortOrder = sortOrder;
            IsActive = isActive;
        }

        public Guid ProductId { get; private set; }

        public Product Product { get; private set; } = default!;

        public Guid ProductVariantId { get; private set; }

        public ProductVariant ProductVariant { get; private set; } = default!;

        public Guid ProviderId { get; private set; }

        public Provider Provider { get; private set; } = default!;

        public Guid CountryId { get; private set; }

        public Country Country { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public string Slug { get; private set; } = default!;

        public string ProviderPackageCode { get; private set; } = default!;

        public decimal? DataAmount { get; private set; }

        public string DataUnit { get; private set; } = default!;

        public int ValidityDays { get; private set; }

        public bool IsUnlimited { get; private set; }

        public string CoverageType { get; private set; } = default!;

        public string? CoverageDescription { get; private set; }

        public string ActivationPolicy { get; private set; } = default!;

        public string? SpeedPolicy { get; private set; }

        public bool HotspotSupported { get; private set; }

        public bool PhoneNumberSupported { get; private set; }

        public bool SmsSupported { get; private set; }

        public bool KycRequired { get; private set; }

        public string QrDeliveryType { get; private set; } = default!;

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        public IReadOnlyCollection<EsimPackageCarrier> Carriers => _carriers;

        public void Update(
             Guid productId,
             Guid productVariantId,
             Guid providerId,
             Guid countryId,
             string name,
             string slug,
             string providerPackageCode,
             decimal? dataAmount,
             string dataUnit,
             int validityDays,
             bool isUnlimited,
             string coverageType,
             string? coverageDescription,
             string activationPolicy,
             string? speedPolicy,
             bool hotspotSupported,
             bool phoneNumberSupported,
             bool smsSupported,
             bool kycRequired,
             string qrDeliveryType,
             int sortOrder,
             bool isActive)
        {
            ProductId = productId;
            ProductVariantId = productVariantId;
            ProviderId = providerId;
            CountryId = countryId;
            Name = name;
            Slug = slug;
            ProviderPackageCode = providerPackageCode;
            DataAmount = dataAmount;
            DataUnit = dataUnit;
            ValidityDays = validityDays;
            IsUnlimited = isUnlimited;
            CoverageType = coverageType;
            CoverageDescription = coverageDescription;
            ActivationPolicy = activationPolicy;
            SpeedPolicy = speedPolicy;
            HotspotSupported = hotspotSupported;
            PhoneNumberSupported = phoneNumberSupported;
            SmsSupported = smsSupported;
            KycRequired = kycRequired;
            QrDeliveryType = qrDeliveryType;
            SortOrder = sortOrder;
            IsActive = isActive;
        }
        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
