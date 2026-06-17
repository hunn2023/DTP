using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Chatbot.Infrastructure.Readers
{
    public sealed class ChatbotCatalogReader : IChatbotCatalogReader
    {
        private readonly CatalogDbContext _context;
        private readonly IConfiguration _configuration;

        public ChatbotCatalogReader(
            CatalogDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IReadOnlyList<ChatbotProductSuggestionDto>> SearchEsimPackagesAsync(
    ChatbotIntentDto intent,
    int take = 3,
    CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var countryKeyword = intent.CountryKeyword?.Trim().ToLowerInvariant();
            var countryCode = intent.CountryCode?.Trim().ToUpperInvariant();

            var usageLevel = intent.UsageLevel?.Trim().ToLowerInvariant();

            var requestedDataAmountInGb = ConvertToGb(
                intent.RequestedDataAmount,
                intent.RequestedDataUnit);

            var query = _context.EsimPackages
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.Product.IsActive &&
                    !x.Product.IsDeleted &&
                    x.ProductVariant.IsActive &&
                    x.Country.IsActive);

            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                query = query.Where(x =>
                    x.Country.Code.ToUpper() == countryCode);
            }
            else if (!string.IsNullOrWhiteSpace(countryKeyword))
            {
                query = query.Where(x =>
                    x.Country.Name.ToLower().Contains(countryKeyword) ||
                    x.Country.Slug.ToLower().Contains(countryKeyword) ||
                    x.Product.Name.ToLower().Contains(countryKeyword) ||
                    x.Name.ToLower().Contains(countryKeyword));
            }

            if (intent.TravelDays.HasValue && intent.TravelDays.Value > 0)
            {
                query = query.Where(x =>
                    x.ValidityDays >= intent.TravelDays.Value);
            }

            if (usageLevel == "unlimited")
            {
                query = query.Where(x => x.IsUnlimited);
            }

            if (requestedDataAmountInGb.HasValue && usageLevel != "unlimited")
            {
                var requestedGb = requestedDataAmountInGb.Value;

                query = query.Where(x =>
                    x.IsUnlimited ||
                    (
                        x.DataAmount.HasValue &&
                        (
                            (x.DataUnit != null &&
                             x.DataUnit.ToUpper() == "GB" &&
                             x.DataAmount.Value >= requestedGb)

                            ||

                            (x.DataUnit != null &&
                             x.DataUnit.ToUpper() == "MB" &&
                             (x.DataAmount.Value / 1024m) >= requestedGb)
                        )
                    ));
            }

            if (intent.NeedsHotspot == true)
            {
                query = query.Where(x => x.HotspotSupported);
            }

            if (intent.NeedsPhoneNumber == true)
            {
                query = query.Where(x => x.PhoneNumberSupported);
            }

            if (intent.NeedsSms == true)
            {
                query = query.Where(x => x.SmsSupported);
            }

            var rows = await query
                .Select(x => new PackageProjection
                {
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    EsimPackageId = x.Id,

                    ProductName = x.Product.Name,
                    ProductSlug = x.Product.Slug,

                    CountryName = x.Country.Name,
                    FlagUrl = x.Country.FlagUrl,

                    PackageName = x.Name,
                    ProviderPackageCode = x.ProviderPackageCode,

                    DataAmount = x.DataAmount,
                    DataUnit = x.DataUnit,
                    IsUnlimited = x.IsUnlimited,

                    ValidityDays = x.ValidityDays,

                    CoverageDescription = x.CoverageDescription,
                    ActivationPolicy = x.ActivationPolicy,
                    SpeedPolicy = x.SpeedPolicy,

                    HotspotSupported = x.HotspotSupported,
                    PhoneNumberSupported = x.PhoneNumberSupported,
                    SmsSupported = x.SmsSupported,

                    SalePrice = _context.ProductPrices
                        .Where(p =>
                            p.ProductId == x.ProductId &&
                            p.IsActive &&
                            p.SalePrice > 0 &&
                            (p.ProductVariantId == x.ProductVariantId || p.ProductVariantId == null) &&
                            (p.StartDate == null || p.StartDate <= now) &&
                            (p.EndDate == null || p.EndDate >= now))
                        .OrderBy(p => p.ProductVariantId == x.ProductVariantId ? 0 : 1)
                        .ThenBy(p => p.SalePrice)
                        .Select(p => (decimal?)p.SalePrice)
                        .FirstOrDefault(),

                    Currency = _context.ProductPrices
                        .Where(p =>
                            p.ProductId == x.ProductId &&
                            p.IsActive &&
                            p.SalePrice > 0 &&
                            (p.ProductVariantId == x.ProductVariantId || p.ProductVariantId == null) &&
                            (p.StartDate == null || p.StartDate <= now) &&
                            (p.EndDate == null || p.EndDate >= now))
                        .OrderBy(p => p.ProductVariantId == x.ProductVariantId ? 0 : 1)
                        .ThenBy(p => p.SalePrice)
                        .Select(p => p.Currency)
                        .FirstOrDefault()
                })
                .Where(x => x.SalePrice != null)
                .Take(50)
                .ToListAsync(cancellationToken);

            var result = rows
                .Select(x =>
                {
                    var dto = new ChatbotProductSuggestionDto
                    {
                        ProductId = x.ProductId,
                        ProductVariantId = x.ProductVariantId,
                        EsimPackageId = x.EsimPackageId,

                        ProductName = x.ProductName,
                        ProductSlug = x.ProductSlug,

                        CountryName = x.CountryName,
                        FlagUrl = x.FlagUrl,

                        PackageName = x.PackageName,
                        ProviderPackageCode = x.ProviderPackageCode,

                        DataAmount = x.DataAmount,
                        DataUnit = NormalizeDataUnit(x.DataUnit),
                        IsUnlimited = x.IsUnlimited,

                        ValidityDays = x.ValidityDays,

                        SalePrice = x.SalePrice ?? 0,
                        Currency = string.IsNullOrWhiteSpace(x.Currency)
                            ? "VND"
                            : x.Currency,

                        CoverageDescription = x.CoverageDescription,
                        ActivationPolicy = x.ActivationPolicy,
                        SpeedPolicy = x.SpeedPolicy,

                        HotspotSupported = x.HotspotSupported,
                        PhoneNumberSupported = x.PhoneNumberSupported,
                        SmsSupported = x.SmsSupported,

                        BuyUrl = BuildBuyUrl(x.ProductSlug)
                    };

                    dto.Score = CalculateScore(dto, intent);

                    return dto;
                })
                .ToList();

            if (string.Equals(intent.BudgetType, "cheapest", StringComparison.OrdinalIgnoreCase))
            {
                return result
                    .OrderBy(x => x.SalePrice)
                    .ThenByDescending(x => x.Score)
                    .ThenByDescending(x => x.IsUnlimited)
                    .Take(take)
                    .ToList();
            }

            if (string.Equals(intent.BudgetType, "premium", StringComparison.OrdinalIgnoreCase))
            {
                return result
                    .OrderByDescending(x => x.Score)
                    .ThenByDescending(x => x.IsUnlimited)
                    .ThenByDescending(x => ConvertPackageDataToGb(x))
                    .ThenBy(x => x.SalePrice)
                    .Take(take)
                    .ToList();
            }

            return result
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.IsUnlimited)
                .ThenBy(x => x.SalePrice)
                .Take(take)
                .ToList();
        }

        private string BuildBuyUrl(string productSlug)
        {
            var baseUrl = _configuration["Chatbot:StorefrontBaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return $"/{productSlug}";
            }

            return $"{baseUrl.TrimEnd('/')}/{productSlug}";
        }

        private static int CalculateScore(
     ChatbotProductSuggestionDto item,
     ChatbotIntentDto intent)
        {
            var score = 0;

            var itemDataGb = ConvertPackageDataToGb(item);

            var requestedDataGb = ConvertToGb(
                intent.RequestedDataAmount,
                intent.RequestedDataUnit);

            if (intent.TravelDays.HasValue)
            {
                var days = intent.TravelDays.Value;

                if (item.ValidityDays >= days)
                    score += 40;

                var diff = item.ValidityDays - days;

                if (diff >= 0 && diff <= 3)
                    score += 20;
                else if (diff > 3 && diff <= 7)
                    score += 10;
            }

            if (requestedDataGb.HasValue)
            {
                var requested = requestedDataGb.Value;

                if (item.IsUnlimited)
                {
                    score += 45;
                }
                else if (itemDataGb.HasValue)
                {
                    var data = itemDataGb.Value;

                    if (data >= requested)
                        score += 35;

                    var diff = data - requested;

                    if (diff >= 0 && diff <= 2)
                        score += 20;
                    else if (diff > 2 && diff <= 5)
                        score += 10;
                }
            }

            var usageLevel = intent.UsageLevel?.Trim().ToLowerInvariant();

            if (usageLevel == "unlimited")
            {
                if (item.IsUnlimited)
                    score += 60;
            }
            else if (usageLevel == "heavy")
            {
                if (item.IsUnlimited)
                    score += 50;
                else if ((itemDataGb ?? 0) >= 10)
                    score += 35;
                else if ((itemDataGb ?? 0) >= 5)
                    score += 20;
            }
            else if (usageLevel == "normal")
            {
                if (item.IsUnlimited)
                    score += 25;
                else if ((itemDataGb ?? 0) >= 5)
                    score += 30;
                else if ((itemDataGb ?? 0) >= 3)
                    score += 20;
            }
            else if (usageLevel == "light")
            {
                if (!item.IsUnlimited && (itemDataGb ?? 0) <= 5)
                    score += 25;
                else if (!item.IsUnlimited)
                    score += 15;
            }

            if (intent.NeedsHotspot == true && item.HotspotSupported)
                score += 10;

            if (intent.NeedsPhoneNumber == true && item.PhoneNumberSupported)
                score += 10;

            if (intent.NeedsSms == true && item.SmsSupported)
                score += 10;

            if (item.HotspotSupported)
                score += 3;

            if (!string.IsNullOrWhiteSpace(item.ActivationPolicy))
                score += 2;

            if (!string.IsNullOrWhiteSpace(item.CoverageDescription))
                score += 2;

            return score;
        }


        private static string? NormalizeDataUnit(string? unit)
        {
            if (string.IsNullOrWhiteSpace(unit))
                return null;

            var value = unit.Trim().ToUpperInvariant();

            return value switch
            {
                "G" => "GB",
                "GB" => "GB",
                "M" => "MB",
                "MB" => "MB",
                _ => value
            };
        }

        private static decimal? ConvertPackageDataToGb(ChatbotProductSuggestionDto item)
        {
            if (item.IsUnlimited)
                return null;

            return ConvertToGb(item.DataAmount, item.DataUnit);
        }

        private static decimal? ConvertToGb(decimal? amount, string? unit)
        {
            if (!amount.HasValue)
                return null;

            if (string.IsNullOrWhiteSpace(unit))
                return amount.Value;

            var normalizedUnit = unit.Trim().ToUpperInvariant();

            return normalizedUnit switch
            {
                "GB" or "G" => amount.Value,
                "MB" or "M" => amount.Value / 1024m,
                _ => amount.Value
            };
        }

        private sealed class PackageProjection
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

            public decimal? SalePrice { get; set; }

            public string? Currency { get; set; }

            public string? CoverageDescription { get; set; }

            public string? ActivationPolicy { get; set; }

            public string? SpeedPolicy { get; set; }

            public bool HotspotSupported { get; set; }

            public bool PhoneNumberSupported { get; set; }

            public bool SmsSupported { get; set; }
        }
    }
}
