using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Shared.Application.Common
{
    public class FileDto
    {
        public Guid? Id { get; set; }

        public string FileName { get; set; } = default!;

        public string FileUrl { get; set; } = default!;

        public string? ContentType { get; set; }

        public long? Size { get; set; }

        public string? AltText { get; set; }

        public int SortOrder { get; set; }

        public bool IsMain { get; set; }
    }
}
