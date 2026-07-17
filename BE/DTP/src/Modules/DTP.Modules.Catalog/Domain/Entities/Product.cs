using Azure.Core;
using DTP.Shared.Domain;


namespace DTP.Modules.Catalog.Domain.Entities
{
    public class Product : EntityBase
    {
        private readonly List<ProductVariant> _variants = new();
        private readonly List<ProductImage> _images = new();
        private readonly List<ProductAttribute> _attributes = new();
        private readonly List<ProductContent> _contents = new();
        private readonly List<ProductFaq> _faqs = new();


        private Product()
        {
        }

        public Product(
            string? code,
            string name,
            string slug,
            Guid categoryId,
            Guid? countryId,
            string? shortDescription,
            string? description,
            string? locationText,
            string? thumbnailUrl,
            bool isFeatured,
            bool isHot,
            int sortOrder,
            bool isActive)
        {
            Code = code;
            Name = name;
            Slug = slug;
            CategoryId = categoryId;
            CountryId = countryId;
            ShortDescription = shortDescription;
            Description = description;
            LocationText = locationText;
            IsFeatured = isFeatured;
            IsHot = isHot;
            SoldCount = 0;
            SortOrder = sortOrder;
            IsActive = isActive;
        }

        public string? Code { get; private set; }

        public string Name { get; private set; } = default!;

        public string Slug { get; private set; } = default!;

        public Guid CategoryId { get; private set; }

        public Guid? CountryId { get; private set; }


        public Category? Category { get; private set; }


        public Country? Country { get; private set; }

        public string? ShortDescription { get; private set; }

        public string? Description { get; private set; }

        public string? LocationText { get; private set; }

        public string? ThumbnailUrl { get; private set; }

        public bool IsFeatured { get; private set; }

        public bool IsHot { get; private set; }

        public int SoldCount { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        public IReadOnlyCollection<ProductVariant> Variants => _variants;

        public IReadOnlyCollection<ProductImage> Images => _images;

        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes;

        public IReadOnlyCollection<ProductContent> Contents => _contents;

        public IReadOnlyCollection<ProductFaq> Faqs => _faqs;

        public void Update(
            string? code,
            string name,
            string slug,
            Guid categoryId,
            Guid? countryId,
            string? shortDescription,
            string? description,
            string? locationText,
            bool isFeatured,
            bool isHot,
            int sortOrder,
            bool isActive)
        {
            Code = code;
            Name = name;
            Slug = slug;
            CategoryId = categoryId;
            CountryId = countryId;
            ShortDescription = shortDescription;
            Description = description;
            LocationText = locationText;
            IsFeatured = isFeatured;
            IsHot = isHot;
            SortOrder = sortOrder;
            IsActive = isActive;
        }

        public void UpdateThumbnail(string? thumbnailUrl)
        {
            ThumbnailUrl = thumbnailUrl;
        }

        public void IncreaseSoldCount(int quantity)
        {
            if (quantity <= 0)
                return;

            SoldCount += quantity;
        }

        public void DecreaseSoldCount(int quantity)
        {
            if (quantity <= 0)
                return;

            SoldCount = Math.Max(0, SoldCount - quantity);
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }


        public void UpdateBasicInfo(string name,string slug,string? description,bool isActive)
        {
            Name = name;
            Slug = slug;
            Description = description;
            IsActive = isActive;
        }


    }
}
