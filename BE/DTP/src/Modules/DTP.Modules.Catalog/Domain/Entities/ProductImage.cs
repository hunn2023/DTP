using DTP.Shared.Domain;

namespace DTP.Modules.Catalog.Domain.Entities
{
    public class ProductImage : EntityBase
    {
        private ProductImage()
        {
        }

        public ProductImage(
            Guid productId,
            string imageUrl,
            string imageKey,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            string? contentType,
            long size,
            bool isActive = true)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            ImageUrl = imageUrl;
            ImageKey = imageKey;
            AltText = altText;
            SortOrder = sortOrder;
            IsThumbnail = isThumbnail;
            ContentType = contentType;
            Size = size;
            IsActive = isActive;
        }

        public Guid ProductId { get; private set; }

        public string ImageUrl { get; private set; } = default!;

        public string ImageKey { get; private set; } = default!;

        public string? AltText { get; private set; }

        public int SortOrder { get; private set; }

        public bool IsThumbnail { get; private set; }

        public string? ContentType { get; private set; }

        public long Size { get; private set; }

        public bool IsActive { get; private set; }

        public void Update(
            string imageUrl,
            string imageKey,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            string? contentType,
            long size,
            bool isActive)
        {
            ImageUrl = imageUrl;
            ImageKey = imageKey;
            AltText = altText;
            SortOrder = sortOrder;
            IsThumbnail = isThumbnail;
            ContentType = contentType;
            Size = size;
            IsActive = isActive;
        }

        public void SetThumbnail()
        {
            IsThumbnail = true;
        }

        public void UnsetThumbnail()
        {
            IsThumbnail = false;
        }

        public void UpdateSortOrder(int sortOrder)
        {
            SortOrder = sortOrder;
        }

        public void Deactivate()
        {
            IsActive = false;
            IsThumbnail = false;
        }

        public void UpdateInfo(string? altText, int sortOrder)
        {
            AltText = altText;
            SortOrder = sortOrder;
        }

        public void ReplaceImage(
            string imageUrl,
            string imageKey,
            string? contentType,
            long size)
        {
            ImageUrl = imageUrl;
            ImageKey = imageKey;
            ContentType = contentType;
            Size = size;
        }
    }
}
