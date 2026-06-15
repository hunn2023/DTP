using DTP.Modules.Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class UpdateProductContentDto
    {
        public ProductContentType ContentType { get; set; }

        public string Title { get; set; } = null!;

        public string? Summary { get; set; }

        public string BodyHtml { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
