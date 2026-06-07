using DTP.Shared.Domain;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductImage : EntityBase
    {
        public Guid ProductId { get; private set; }

        public string ImageUrl { get; private set; } = default!;

        public string ImageKey { get; private set; } = default!;

        public string? AltText { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsThumbnail { get; private set; }

        private ProductImage() { }

        public ProductImage(
            Guid productId,
            string imageUrl,
            string imageKey,
            string? altText,
            int sortOrder,
            bool isThumbnail)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            ImageUrl = imageUrl.Trim();
            ImageKey = imageKey;
            AltText = altText?.Trim();
            SortOrder = sortOrder;
            IsThumbnail = isThumbnail;
        }

        public ProductImage(Guid productId, string imageUrl, string? altText, int sortOrder, bool isThumbnail)
        {
            ProductId = productId;
            ImageUrl = imageUrl;
            AltText = altText;
            SortOrder = sortOrder;
            IsThumbnail = isThumbnail;
        }

        public void Update(
                string imageUrl,
                string imageKey,
                string? altText,
                int sortOrder,
                bool isThumbnail)
        {
            ImageUrl = imageUrl.Trim();
            ImageKey = imageKey;
            AltText = altText?.Trim();
            SortOrder = sortOrder;
            IsThumbnail = isThumbnail;

            SetUpdated();
        }
    }
}
