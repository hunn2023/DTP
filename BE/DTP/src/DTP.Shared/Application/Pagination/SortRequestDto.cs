using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Pagination
{
    public class SortRequestDto : PagedRequestDto
    {
        public string? SortBy { get; set; }

        public string? SortDirection { get; set; } = "asc";

        public bool IsDescending =>
            string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
    }
}
