using DTP.Modules.Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class CreateProductContentRequest
    {
        public Guid ProductId { get; set; }

        public ProductContentType ContentType { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string BodyHtml { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateProductContentRequest
    {
        public ProductContentType ContentType { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Summary { get; set; }

        public string BodyHtml { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
