using DTP.Shared.Domain;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class PhoneCard : EntityBase
    {
        public Guid ProductVariantId { get; private set; }
        public ProductVariant ProductVariant { get; private set; } = default!;

        public Guid ProviderId { get; private set; }

        public string Name { get; private set; } = default!;
        public string Slug { get; private set; } = default!;

        public decimal FaceValue { get; private set; }
        public decimal Price { get; private set; }

        public string Currency { get; private set; } = "VND";

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        private PhoneCard()
        {
        }

        public PhoneCard(
            Guid productVariantId,
            Guid providerId,
            string name,
            string slug,
            decimal faceValue,
            decimal price,
            string currency,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            ProductVariantId = productVariantId;
            ProviderId = providerId;
            Name = name.Trim();
            Slug = slug.Trim().ToLower();
            FaceValue = faceValue;
            Price = price;
            Currency = currency.Trim().ToUpper();
            SortOrder = sortOrder;
            IsActive = true;
        }

        public void Update(
            Guid providerId,
            string name,
            string slug,
            decimal faceValue,
            decimal price,
            string currency,
            int sortOrder,
            bool isActive)
        {
            ProviderId = providerId;
            Name = name.Trim();
            Slug = slug.Trim().ToLower();
            FaceValue = faceValue;
            Price = price;
            Currency = currency.Trim().ToUpper();
            SortOrder = sortOrder;
            IsActive = isActive;

            SetUpdated();
        }
    }
}
