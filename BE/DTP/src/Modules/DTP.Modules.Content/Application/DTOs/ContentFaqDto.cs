using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Application.DTOs
{
    public class ContentFaqDto
    {
        public Guid Id { get; set; }
        public string Question { get; set; } = default!;
        public string Answer { get; set; } = default!;
        public string? CategoryCode { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
