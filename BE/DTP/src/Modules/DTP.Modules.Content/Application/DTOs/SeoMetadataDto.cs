using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.DTOs
{
    public class SeoMetadataDto
    {
        public Guid Id { get; set; }

        public string EntityType { get; set; } = default!;

        public Guid? EntityId { get; set; }

        public string? RoutePath { get; set; }

        public string MetaTitle { get; set; } = default!;

        public string? MetaDescription { get; set; }

        public string? MetaKeywords { get; set; }

        public string? CanonicalUrl { get; set; }

        public string? OgTitle { get; set; }

        public string? OgDescription { get; set; }

        public string? OgImageUrl { get; set; }

        public string Robots { get; set; } = "index,follow";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
