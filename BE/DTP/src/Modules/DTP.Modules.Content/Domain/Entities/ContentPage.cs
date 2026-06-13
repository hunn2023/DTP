using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Domain.Entities
{
    public class ContentPage : EntityBase
    {
        private ContentPage()
        {
        }

        public ContentPage(
            string code,
            string title,
            string slug,
            string? summary,
            string content,
            ContentPageStatus status,
            int sortOrder)
        {
            Code = code.Trim();
            Title = title.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Summary = summary?.Trim();
            Content = content;
            Status = status;
            SortOrder = sortOrder;
        }

        public string Code { get; private set; } = default!;
        public string Title { get; private set; } = default!;
        public string Slug { get; private set; } = default!;
        public string? Summary { get; private set; }
        public string Content { get; private set; } = default!;
        public string? ThumbnailUrl { get; private set; }
        public ContentPageStatus Status { get; private set; }
        public int SortOrder { get; private set; }
        public DateTime? PublishedAt { get; private set; }
       public bool IsPublished => Status == ContentPageStatus.Published;

        public void Update(
            string title,
            string slug,
            string? summary,
            string content,
            string? thumbnailUrl,
            ContentPageStatus status,
            int sortOrder)
        {
            Title = title.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Summary = summary?.Trim();
            Content = content;
            ThumbnailUrl = thumbnailUrl?.Trim();
            SortOrder = sortOrder;

            if (Status != ContentPageStatus.Published && status == ContentPageStatus.Published)
            {
                PublishedAt = DateTime.UtcNow;
            }

            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Hide()
        {
            Status = ContentPageStatus.Hidden;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            Status = ContentPageStatus.Published;
            PublishedAt ??= DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
