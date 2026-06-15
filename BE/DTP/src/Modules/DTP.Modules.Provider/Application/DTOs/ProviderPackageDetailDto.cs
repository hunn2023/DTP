using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderPackageDetailDto : ProviderPackageDto
    {
        public string RawPackageJson { get; set; } = default!;
        public string? RawDetailJson { get; set; }

        public ProviderProductMappingDto? Mapping { get; set; }
    }
}
