using DTP.Modules.Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductContentDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public ProductContentType ContentType { get; set; }

        public string ContentTypeName => ContentType.ToString();

        public string Title { get; set; } = null!;

        public string? Summary { get; set; }

        public string BodyHtml { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
