using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.DTOs
{
    public class ContentArticleDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Summary { get; set; }
        public string Content { get; set; } = default!;
        public string? ThumbnailUrl { get; set; }
        public string? AuthorName { get; set; }
        public string? CategoryCode { get; set; }
        public string? Tags { get; set; }
        public ContentArticleStatus Status { get; set; }
        public bool IsFeatured { get; set; }
        public int SortOrder { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
