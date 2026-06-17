using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Application.DTOs
{
    public sealed class ChatbotProductSuggestionDto
    {
        public Guid ProductId { get; set; }

        public Guid ProductVariantId { get; set; }

        public Guid EsimPackageId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string ProductSlug { get; set; } = string.Empty;

        public string CountryName { get; set; } = string.Empty;

        public string? FlagUrl { get; set; }

        public string PackageName { get; set; } = string.Empty;

        public string ProviderPackageCode { get; set; } = string.Empty;

        public decimal? DataAmount { get; set; }

        public string? DataUnit { get; set; }

        public bool IsUnlimited { get; set; }

        public int ValidityDays { get; set; }

        public decimal SalePrice { get; set; }

        public string Currency { get; set; } = "VND";

        public string? CoverageDescription { get; set; }

        public string? ActivationPolicy { get; set; }

        public string? SpeedPolicy { get; set; }

        public bool HotspotSupported { get; set; }

        public bool PhoneNumberSupported { get; set; }

        public bool SmsSupported { get; set; }

        public int Score { get; set; }

        public string BuyUrl { get; set; } = string.Empty;
    }
}
