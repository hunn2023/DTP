using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderCoverageCountryDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? LogoUrl { get; set; }

        public List<string> Operators { get; set; } = new();
        public List<string> NetworkTypes { get; set; } = new();
    }
}
