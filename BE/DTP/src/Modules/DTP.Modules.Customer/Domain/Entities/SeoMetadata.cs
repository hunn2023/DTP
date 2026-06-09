using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Domain.Entities
{
    public class SeoMetadata : EntityBase
    {
        private SeoMetadata()
        {
        }

        public SeoMetadata(
            string entityType,
            Guid? entityId,
            string? routePath,
            string metaTitle,
            string? metaDescription,
            string? metaKeywords,
            string? canonicalUrl,
            string? ogTitle,
            string? ogDescription,
            string? ogImageUrl,
            string? robots)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("EntityType is required.");

            if (!entityId.HasValue && string.IsNullOrWhiteSpace(routePath))
                throw new ArgumentException("EntityId or RoutePath is required.");

            if (string.IsNullOrWhiteSpace(metaTitle))
                throw new ArgumentException("Meta title is required.");

            Id = Guid.NewGuid();
            EntityType = NormalizeEntityType(entityType);
            EntityId = entityId;
            RoutePath = NormalizeRoutePath(routePath);

            MetaTitle = metaTitle.Trim();
            MetaDescription = metaDescription?.Trim();
            MetaKeywords = metaKeywords?.Trim();
            CanonicalUrl = canonicalUrl?.Trim();

            OgTitle = ogTitle?.Trim();
            OgDescription = ogDescription?.Trim();
            OgImageUrl = ogImageUrl?.Trim();

            Robots = string.IsNullOrWhiteSpace(robots)
                ? "index,follow"
                : robots.Trim();

            CreatedAt = DateTime.UtcNow;
        }

        public string EntityType { get; private set; } = default!;

        public Guid? EntityId { get; private set; }

        public string? RoutePath { get; private set; }

        public string MetaTitle { get; private set; } = default!;

        public string? MetaDescription { get; private set; }

        public string? MetaKeywords { get; private set; }

        public string? CanonicalUrl { get; private set; }

        public string? OgTitle { get; private set; }

        public string? OgDescription { get; private set; }

        public string? OgImageUrl { get; private set; }

        public string Robots { get; private set; } = "index,follow";

        public DateTime CreatedAt { get; private set; }

        public DateTime? UpdatedAt { get; private set; }

        public void Update(
            string metaTitle,
            string? metaDescription,
            string? metaKeywords,
            string? canonicalUrl,
            string? ogTitle,
            string? ogDescription,
            string? ogImageUrl,
            string? robots)
        {
            if (string.IsNullOrWhiteSpace(metaTitle))
                throw new ArgumentException("Meta title is required.");

            MetaTitle = metaTitle.Trim();
            MetaDescription = metaDescription?.Trim();
            MetaKeywords = metaKeywords?.Trim();
            CanonicalUrl = canonicalUrl?.Trim();

            OgTitle = ogTitle?.Trim();
            OgDescription = ogDescription?.Trim();
            OgImageUrl = ogImageUrl?.Trim();

            Robots = string.IsNullOrWhiteSpace(robots)
                ? "index,follow"
                : robots.Trim();

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateTarget(
            string entityType,
            Guid? entityId,
            string? routePath)
        {
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("EntityType is required.");

            if (!entityId.HasValue && string.IsNullOrWhiteSpace(routePath))
                throw new ArgumentException("EntityId or RoutePath is required.");

            EntityType = NormalizeEntityType(entityType);
            EntityId = entityId;
            RoutePath = NormalizeRoutePath(routePath);
            UpdatedAt = DateTime.UtcNow;
        }

        private static string NormalizeEntityType(string entityType)
        {
            return entityType.Trim();
        }

        private static string? NormalizeRoutePath(string? routePath)
        {
            if (string.IsNullOrWhiteSpace(routePath))
                return null;

            routePath = routePath.Trim();

            if (!routePath.StartsWith("/"))
                routePath = "/" + routePath;

            return routePath.ToLowerInvariant();
        }
    }
}
