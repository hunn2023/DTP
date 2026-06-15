using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderPackageDto
    {
        public Guid Id { get; set; }
        public Guid ProviderId { get; set; }

        public string ProviderSku { get; set; } = default!;
        public string? ProviderProductId { get; set; }

        public string Name { get; set; } = default!;
        public string? Model { get; set; }

        public string? Regional { get; set; }
        public int? RegionId { get; set; }

        public decimal Price { get; set; }
        public string CurrencyCode { get; set; } = default!;

        public string SyncStatus { get; set; } = default!;
        public string? ErrorMessage { get; set; }

        public DateTime LastSyncedAt { get; set; }
    }
}
