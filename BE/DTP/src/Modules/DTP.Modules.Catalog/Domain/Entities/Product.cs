using DTP.Shared.Domain;


namespace DTP.Modules.Catalog.Domain.Entities
{
    public class Product : EntityBase
    {
        public string? Code { get; private set; }

        public string Name { get; private set; } = default!;

        public string Slug { get; private set; } = default!;

        public Guid CategoryId { get; private set; }

        public string? ShortDescription { get; private set; }

        public string? Description { get; private set; }

        public string? ThumbnailUrl { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        private readonly List<ProductVariant> _variants = new();
        public IReadOnlyCollection<ProductVariant> Variants => _variants;

        private readonly List<ProductImage> _images = new();
        public IReadOnlyCollection<ProductImage> Images => _images;

        private readonly List<ProductAttribute> _attributes = new();
        public IReadOnlyCollection<ProductAttribute> Attributes => _attributes;

        private Product() { }

        public Product(
            string? code,
            string name,
            string slug,
            Guid categoryId,
            string? shortDescription,
            string? description,
            string? thumbnailUrl,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            Code = code?.Trim();
            Name = name.Trim();
            Slug = slug.Trim();
            CategoryId = categoryId;
            ShortDescription = shortDescription?.Trim();
            Description = description?.Trim();
            ThumbnailUrl = thumbnailUrl?.Trim();
            SortOrder = sortOrder;
            IsActive = true;
        }

        public void Update(
            string? code,
            string name,
            string slug,
            Guid categoryId,
            string? shortDescription,
            string? description,
            string? thumbnailUrl,
            int sortOrder,
            bool isActive)
        {
            Code = code?.Trim();
            Name = name.Trim();
            Slug = slug.Trim();
            CategoryId = categoryId;
            ShortDescription = shortDescription?.Trim();
            Description = description?.Trim();
            ThumbnailUrl = thumbnailUrl?.Trim();
            SortOrder = sortOrder;
            IsActive = isActive;

            SetUpdated();
        }
    }
}
