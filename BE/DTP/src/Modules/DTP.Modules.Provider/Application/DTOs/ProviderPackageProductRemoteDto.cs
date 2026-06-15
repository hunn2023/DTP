using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderPackageProductRemoteDto
    {
        public string? Id { get; set; }
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Model { get; set; }

        public string? Regional { get; set; }
        public int? RegionId { get; set; }

        public decimal Price { get; set; }
        public string CurrencyCode { get; set; } = "VND";

        public string? ImageUrl { get; set; }

        public string RawJson { get; set; } = default!;
    }
}
