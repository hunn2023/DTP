using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Application.DTOs
{
    public class ProviderEsimProductRemoteDto
    {
        public string Sku { get; set; } = default!;
        public string Name { get; set; } = default!;

        public decimal Price { get; set; }
        public string CurrencyCode { get; set; } = "VND";

        public decimal? DataAmount { get; set; }
        public string? DataUnit { get; set; }
        public int ValidityDays { get; set; }
        public bool IsUnlimited { get; set; }

        public string? CoverageType { get; set; }
        public string? CoverageDescription { get; set; }

        public string? ActivationPolicy { get; set; }
        public string? SpeedPolicy { get; set; }

        public bool HotspotSupported { get; set; }
        public bool PhoneNumberSupported { get; set; }
        public bool SmsSupported { get; set; }
        public bool KycRequired { get; set; }

        public List<ProviderCoverageCountryDto> Countries { get; set; } = new();
        public List<string> Operators { get; set; } = new();

        public string RawJson { get; set; } = default!;
    }
}
