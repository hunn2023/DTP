using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class CreateProductFaqRequest
    {
        public Guid ProductId { get; set; }

        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateProductFaqRequest
    {
        public string Question { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
