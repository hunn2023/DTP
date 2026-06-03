using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }

        public Guid? ProductId { get; set; }
        public string ImageUrl { get; set; } = default!;
        public string? AltText { get; set; }
        public int SortOrder { get; set; }
        public bool IsThumbnail { get; set; }
    }
}
