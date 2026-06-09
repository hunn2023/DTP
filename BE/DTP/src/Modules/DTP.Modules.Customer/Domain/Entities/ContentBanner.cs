using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Domain.Entities
{
    public class ContentBanner : EntityBase
    {
        private ContentBanner()
        {
        }

        public ContentBanner(
            string title,
            string imageUrl,
            string? mobileImageUrl,
            string? linkUrl,
            string? description,
            ContentBannerPosition position,
            DateTime? startDate,
            DateTime? endDate,
            int sortOrder,
            bool isActive)
        {
            Id = Guid.NewGuid();
            Title = title.Trim();
            ImageUrl = imageUrl.Trim();
            MobileImageUrl = mobileImageUrl?.Trim();
            LinkUrl = linkUrl?.Trim();
            Description = description?.Trim();
            Position = position;
            StartDate = startDate;
            EndDate = endDate;
            SortOrder = sortOrder;
            IsActive = isActive;
            CreatedAt = DateTime.UtcNow;
        }

        public string Title { get; private set; } = default!;
        public string ImageUrl { get; private set; } = default!;
        public string? MobileImageUrl { get; private set; }
        public string? LinkUrl { get; private set; }
        public string? Description { get; private set; }
        public ContentBannerPosition Position { get; private set; }

        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }

        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public void Update(
            string title,
            string imageUrl,
            string? mobileImageUrl,
            string? linkUrl,
            string? description,
            ContentBannerPosition position,
            DateTime? startDate,
            DateTime? endDate,
            int sortOrder,
            bool isActive)
        {
            Title = title.Trim();
            ImageUrl = imageUrl.Trim();
            MobileImageUrl = mobileImageUrl?.Trim();
            LinkUrl = linkUrl?.Trim();
            Description = description?.Trim();
            Position = position;
            StartDate = startDate;
            EndDate = endDate;
            SortOrder = sortOrder;
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsAvailableNow(DateTime now)
        {
            if (!IsActive)
                return false;

            if (StartDate.HasValue && StartDate.Value > now)
                return false;

            if (EndDate.HasValue && EndDate.Value < now)
                return false;

            return true;
        }

        public void Disable()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
