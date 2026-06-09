using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.DTOs
{
    public class ContentBannerDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
        public string? MobileImageUrl { get; set; }
        public string? LinkUrl { get; set; }
        public string? Description { get; set; }
        public ContentBannerPosition Position { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
