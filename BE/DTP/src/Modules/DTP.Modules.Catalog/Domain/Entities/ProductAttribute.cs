using DTP.Shared.Domain;


namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductAttribute : EntityBase
    {
        private ProductAttribute()
        {
        }

        public ProductAttribute(
            Guid productId,
            string key,
            string? displayName,
            string value,
            int sortOrder,
            bool isVisible)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            Key = key;
            DisplayName = displayName;
            Value = value;
            SortOrder = sortOrder;
            IsVisible = isVisible;
        }

        public Guid ProductId { get; private set; }

        public string Key { get; private set; } = default!;

        public string? DisplayName { get; private set; }

        public string Value { get; private set; } = default!;

        public int SortOrder { get; private set; }

        public bool IsVisible { get; private set; }

        public void Update(
            string key,
            string? displayName,
            string value,
            int sortOrder,
            bool isVisible)
        {
            Key = key;
            DisplayName = displayName;
            Value = value;
            SortOrder = sortOrder;
            IsVisible = isVisible;
        }
    }
}
