using DTP.Modules.Provider.Domain.Enums.DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ExternalProviderDto
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public ProviderType Type { get; set; }

        public string? ApiBaseUrl { get; set; }

        public bool IsActive { get; set; }

        public string? Description { get; set; }
    }
}
