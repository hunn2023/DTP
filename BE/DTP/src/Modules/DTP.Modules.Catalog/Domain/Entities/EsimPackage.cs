using DTP.Shared.Domain;


namespace DTP.Modules.Catalog.Domain.Entities
{
    public class EsimPackage : EntityBase
    {
        public Guid ProductVariantId { get; private set; }
        public ProductVariant ProductVariant { get; private set; } = default!;

        public Guid CountryId { get; private set; }
        public Country Country { get; private set; } = default!;

        public Guid CarrierId { get; private set; }
        public Carrier Carrier { get; private set; } = default!;

        public string Name { get; private set; } = default!;

        public string Slug { get; private set; } = default!;

        public int DataAmount { get; private set; }

        public string DataUnit { get; private set; } = default!;

        public int ValidityDays { get; private set; }

        public decimal Price { get; private set; }

        public string Currency { get; private set; } = "USD";

        public bool IsUnlimited { get; private set; }

        public bool IsActive { get; private set; }

        public int SortOrder { get; private set; }

        private EsimPackage()
        {
        }

        public EsimPackage(
            Guid productVariantId,
            Guid countryId,
            Guid carrierId,
            string name,
            string slug,
            int dataAmount,
            string dataUnit,
            int validityDays,
            decimal price,
            string currency,
            bool isUnlimited,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            ProductVariantId = productVariantId;
            CountryId = countryId;
            CarrierId = carrierId;
            Name = name.Trim();
            Slug = slug.Trim().ToLower();
            DataAmount = dataAmount;
            DataUnit = dataUnit.Trim();
            ValidityDays = validityDays;
            Price = price;
            Currency = currency.Trim().ToUpper();
            IsUnlimited = isUnlimited;
            SortOrder = sortOrder;
            IsActive = true;
        }

        public void Update(
            Guid countryId,
            Guid carrierId,
            string name,
            string slug,
            int dataAmount,
            string dataUnit,
            int validityDays,
            decimal price,
            string currency,
            bool isUnlimited,
            int sortOrder,
            bool isActive)
        {
            CountryId = countryId;
            CarrierId = carrierId;
            Name = name.Trim();
            Slug = slug.Trim().ToLower();
            DataAmount = dataAmount;
            DataUnit = dataUnit.Trim();
            ValidityDays = validityDays;
            Price = price;
            Currency = currency.Trim().ToUpper();
            IsUnlimited = isUnlimited;
            SortOrder = sortOrder;
            IsActive = isActive;

            SetUpdated();
        }
    }
}
