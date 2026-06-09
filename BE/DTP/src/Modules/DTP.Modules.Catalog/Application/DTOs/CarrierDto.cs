using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Application.DTOs
{
    public class CarrierDto
    {
        public Guid Id { get; set; }

        public string? Code { get; set; }

        public string Name { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public Guid CountryId { get; set; }

        public string? LogoUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
