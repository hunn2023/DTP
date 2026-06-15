using DTP.Shared.Domain;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductVariant : EntityBase
    {
        private ProductVariant()
        {
        }

        public ProductVariant(
            Guid productId,
            string? sku,
            string name,
            string? shortName,
            string? description,
            int sortOrder,
            bool isActive)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Sku = sku;
            Name = name;
            ShortName = shortName;
            Description = description;
            SortOrder = sortOrder;
            IsActive = isActive;
        }

        public Guid ProductId { get; private set; }

        public Product? Product { get; private set; }

        public string? Sku { get; private set; }

        public string Name { get; private set; } = default!;

        public string? ShortName { get; private set; }

        public string? Description { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsActive { get; private set; }

        public void Update(
            string? sku,
            string name,
            string? shortName,
            string? description,
            int sortOrder,
            bool isActive)
        {
            Sku = sku;
            Name = name;
            ShortName = shortName;
            Description = description;
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


        public void Update(
          string name,
          string? shortName,
          string? description,
          int sortOrder,
          bool isActive)
        {
            Name = name;
            ShortName = shortName;
            Description = description;
            SortOrder = sortOrder;
            IsActive = isActive;
        }
    }
}
