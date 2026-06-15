using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class ProductFaqDto
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string Question { get; set; } = null!;

        public string Answer { get; set; } = null!;

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
