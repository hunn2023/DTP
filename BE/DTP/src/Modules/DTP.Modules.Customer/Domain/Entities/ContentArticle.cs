using DTP.Modules.Content.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Domain.Entities
{
    public class ContentArticle : EntityBase
    {
        private ContentArticle()
        {
        }

        public ContentArticle(
            string title,
            string slug,
            string? summary,
            string content,
            string? thumbnailUrl,
            string? authorName,
            string? categoryCode,
            string? tags,
            ContentArticleStatus status,
            bool isFeatured,
            int sortOrder)
        {
            Id = Guid.NewGuid();
            Title = title.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Summary = summary?.Trim();
            Content = content;
            ThumbnailUrl = thumbnailUrl?.Trim();
            AuthorName = authorName?.Trim();
            CategoryCode = categoryCode?.Trim();
            Tags = tags?.Trim();
            Status = status;
            IsFeatured = isFeatured;
            SortOrder = sortOrder;
            ViewCount = 0;
            CreatedAt = DateTime.UtcNow;

            if (status == ContentArticleStatus.Published)
                PublishedAt = DateTime.UtcNow;
        }

        public string Title { get; private set; } = default!;
        public string Slug { get; private set; } = default!;
        public string? Summary { get; private set; }
        public string Content { get; private set; } = default!;
        public string? ThumbnailUrl { get; private set; }
        public string? AuthorName { get; private set; }
        public string? CategoryCode { get; private set; }
        public string? Tags { get; private set; }

        public ContentArticleStatus Status { get; private set; }
        public bool IsFeatured { get; private set; }
        public int SortOrder { get; private set; }
        public int ViewCount { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public DateTime? PublishedAt { get; private set; }

        public bool IsPublished => Status == ContentArticleStatus.Published;

        public void Update(
            string title,
            string slug,
            string? summary,
            string content,
            string? thumbnailUrl,
            string? authorName,
            string? categoryCode,
            string? tags,
            ContentArticleStatus status,
            bool isFeatured,
            int sortOrder)
        {
            Title = title.Trim();
            Slug = slug.Trim().ToLowerInvariant();
            Summary = summary?.Trim();
            Content = content;
            ThumbnailUrl = thumbnailUrl?.Trim();
            AuthorName = authorName?.Trim();
            CategoryCode = categoryCode?.Trim();
            Tags = tags?.Trim();
            IsFeatured = isFeatured;
            SortOrder = sortOrder;

            if (Status != ContentArticleStatus.Published && status == ContentArticleStatus.Published)
            {
                PublishedAt = DateTime.UtcNow;
            }

            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncreaseView()
        {
            ViewCount++;
        }

        public void Hide()
        {
            Status = ContentArticleStatus.Hidden;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            Status = ContentArticleStatus.Published;
            PublishedAt ??= DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
