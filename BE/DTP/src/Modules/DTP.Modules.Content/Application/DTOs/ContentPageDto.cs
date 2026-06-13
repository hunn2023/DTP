using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.DTOs
{
    public class ContentPageDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Summary { get; set; }
        public string Content { get; set; } = default!;
        public string? ThumbnailUrl { get; set; }
        public ContentPageStatus Status { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}
