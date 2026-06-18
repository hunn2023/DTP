using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Modules.Chatbot.Application.Abstractions;
using DTP.Modules.Chatbot.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
            if (intent == null)
                return Array.Empty<ChatbotProductSuggestionDto>();

            if (take <= 0)
                take = 3;

            var now = DateTime.UtcNow;

            var countryKeyword = intent.CountryKeyword?.Trim().ToLowerInvariant();
            var countryCode = intent.CountryCode?.Trim().ToUpperInvariant();
            var usageLevel = intent.UsageLevel?.Trim().ToLowerInvariant();

            // Bắt buộc phải có quốc gia.
            // Nếu không có quốc gia mà vẫn query thì sẽ lấy lung tung Nhật, Thái, Hàn...
            if (string.IsNullOrWhiteSpace(countryCode)
                && string.IsNullOrWhiteSpace(countryKeyword))
            {
                return Array.Empty<ChatbotProductSuggestionDto>();
            }

            var requestedDataAmountInGb = ConvertToGb(
                intent.RequestedDataAmount,
                intent.RequestedDataUnit);


            var countryId = await ResolveCountryIdAsync(
                    intent,
                    cancellationToken);

            if (!countryId.HasValue)
            {
                return Array.Empty<ChatbotProductSuggestionDto>();
            }


            var query = _context.EsimPackages
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive && !x.IsDeleted &&
                        x.CountryId == countryId.Value &&
                        x.Product.IsActive &&
                        !x.Product.IsDeleted &&
                        x.ProductVariant.IsActive &&
                        x.Country.IsActive);

            //var query = _context.EsimPackages
            //    .AsNoTracking()
            //    .Where(x =>
            //        x.IsActive &&
            //        x.Product.IsActive &&
            //        !x.Product.IsDeleted &&
            //        x.ProductVariant.IsActive &&
            //        x.Country.IsActive);

            //if (!string.IsNullOrWhiteSpace(countryCode))
            //{
            //    query = query.Where(x =>
            //        x.Country.Code.ToUpper() == countryCode);
            //}
            //else if (!string.IsNullOrWhiteSpace(countryKeyword))
            //{
            //    query = query.Where(x =>
            //        x.Country.Name.ToLower().Contains(countryKeyword) ||
            //        x.Country.Slug.ToLower().Contains(countryKeyword) ||
            //        x.Country.Code.ToLower().Contains(countryKeyword) ||
            //        x.Product.Name.ToLower().Contains(countryKeyword) ||
            //        x.Name.ToLower().Contains(countryKeyword));
            //}

            if (intent.TravelDays.HasValue && intent.TravelDays.Value > 0)
            {
                query = query.Where(x =>
                    x.ValidityDays >= intent.TravelDays.Value);
            }

            // Nếu khách nói rõ muốn không giới hạn thì chỉ lấy gói IsUnlimited.
            if (usageLevel == "unlimited")
            {
                query = query.Where(x => x.IsUnlimited);
            }

            // Nếu khách hỏi 3GB, 5GB, 10GB...
            // Cho phép lấy:
            // - Gói unlimited
            // - Gói có DataAmount >= requested
            // - Nếu DataUnit null thì mặc định hiểu là GB
            if (requestedDataAmountInGb.HasValue && usageLevel != "unlimited")
            {
                var requestedGb = requestedDataAmountInGb.Value;

                query = query.Where(x =>
                    x.IsUnlimited ||
                    (
                        x.DataAmount.HasValue &&
                        (
                            (
                                (x.DataUnit == null ||
                                 x.DataUnit == "" ||
                                 x.DataUnit.ToUpper() == "GB" ||
                                 x.DataUnit.ToUpper() == "G") &&
                                x.DataAmount.Value >= requestedGb
                            )
                            ||
                            (
                                (x.DataUnit.ToUpper() == "MB" ||
                                 x.DataUnit.ToUpper() == "M") &&
                                (x.DataAmount.Value / 1024m) >= requestedGb
                            )
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

            var packageRows = await query
                .OrderBy(x => x.ValidityDays)
                .ThenBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .Select(x => new PackageProjection
                {
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    EsimPackageId = x.Id,

                    ProductName = x.Product.Name,
                    ProductSlug = x.Product.Slug,

                    CountryCode = x.Country.Code,
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
                    SmsSupported = x.SmsSupported
                })
                .Take(200)
                .ToListAsync(cancellationToken);

            if (packageRows.Count == 0)
                return Array.Empty<ChatbotProductSuggestionDto>();

            var productIds = packageRows
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

            var variantIds = packageRows
                .Select(x => x.ProductVariantId)
                .Distinct()
                .ToList();

            var prices = await _context.ProductPrices
                .AsNoTracking()
                .Where(p =>
                    productIds.Contains(p.ProductId) &&
                    p.IsActive &&
                    p.SalePrice > 0 &&
                    (p.ProductVariantId == null || variantIds.Contains(p.ProductVariantId.Value)) &&
                    (p.StartDate == null || p.StartDate <= now) &&
                    (p.EndDate == null || p.EndDate >= now))
                .Select(p => new PriceProjection
                {
                    ProductId = p.ProductId,
                    ProductVariantId = p.ProductVariantId,
                    SalePrice = p.SalePrice,
                    Currency = p.Currency
                })
                .ToListAsync(cancellationToken);

            var result = new List<ChatbotProductSuggestionDto>();

            foreach (var row in packageRows)
            {
                var price = prices
                    .Where(p =>
                        p.ProductId == row.ProductId &&
                        (
                            p.ProductVariantId == row.ProductVariantId ||
                            p.ProductVariantId == null
                        ))
                    .OrderBy(p =>
                        p.ProductVariantId == row.ProductVariantId
                            ? 0
                            : 1)
                    .ThenBy(p => p.SalePrice)
                    .FirstOrDefault();

                if (price == null)
                    continue;

                var dto = new ChatbotProductSuggestionDto
                {
                    ProductId = row.ProductId,
                    ProductVariantId = row.ProductVariantId,
                    EsimPackageId = row.EsimPackageId,

                    ProductName = row.ProductName,
                    ProductSlug = row.ProductSlug,

                    CountryName = GetCountryDisplayName(
                        row.CountryName,
                        row.CountryCode,
                        row.CoverageDescription),

                    FlagUrl = row.FlagUrl,

                    PackageName = row.PackageName,
                    ProviderPackageCode = row.ProviderPackageCode,

                    DataAmount = row.DataAmount,
                    DataUnit = NormalizeDataUnit(row.DataUnit),
                    IsUnlimited = row.IsUnlimited,

                    ValidityDays = row.ValidityDays,

                    SalePrice = price.SalePrice,
                    Currency = string.IsNullOrWhiteSpace(price.Currency)
                        ? "VND"
                        : price.Currency,

                    CoverageDescription = row.CoverageDescription,
                    ActivationPolicy = row.ActivationPolicy,
                    SpeedPolicy = row.SpeedPolicy,

                    HotspotSupported = row.HotspotSupported,
                    PhoneNumberSupported = row.PhoneNumberSupported,
                    SmsSupported = row.SmsSupported,

                    BuyUrl = BuildBuyUrl(row.ProductSlug)
                };

                dto.Score = CalculateScore(dto, intent);

                result.Add(dto);
            }

            if (result.Count == 0)
                return Array.Empty<ChatbotProductSuggestionDto>();

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
                    .ThenByDescending(x => ConvertPackageDataToGb(x) ?? decimal.MaxValue)
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


        private async Task<Guid?> ResolveCountryIdAsync(
            ChatbotIntentDto intent,
            CancellationToken cancellationToken)
        {
            var countryCode = intent.CountryCode?.Trim().ToUpperInvariant();
            var countryKeyword = intent.CountryKeyword?.Trim().ToLowerInvariant();

            if (!string.IsNullOrWhiteSpace(countryCode))
            {
                var countryId = await _context.Countries
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        !x.IsDeleted &&
                        x.Code.ToUpper() == countryCode)
                    .Select(x => (Guid?)x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (countryId.HasValue)
                    return countryId;
            }

            if (!string.IsNullOrWhiteSpace(countryKeyword))
            {
                var countryId = await _context.Countries
                    .AsNoTracking()
                    .Where(x =>
                        x.IsActive &&
                        !x.IsDeleted &&
                        (
                            x.Name.ToLower().Contains(countryKeyword) ||
                            x.Slug.ToLower().Contains(countryKeyword) ||
                            x.Code.ToLower().Contains(countryKeyword)
                        ))
                    .Select(x => (Guid?)x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (countryId.HasValue)
                    return countryId;
            }

            return null;
        }

        private string BuildBuyUrl(string productSlug)
        {
            var baseUrl = _configuration["Chatbot:StorefrontBaseUrl"]?.TrimEnd('/');
            var productDetailPath = _configuration["Chatbot:ProductDetailPath"]?.Trim('/');

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return string.IsNullOrWhiteSpace(productDetailPath)
                    ? $"/{productSlug}"
                    : $"/{productDetailPath}/{productSlug}";
            }

            if (string.IsNullOrWhiteSpace(productDetailPath))
            {
                return $"{baseUrl}/{productSlug}";
            }

            if (baseUrl.EndsWith($"/{productDetailPath}", StringComparison.OrdinalIgnoreCase))
            {
                return $"{baseUrl}/{productSlug}";
            }

            return $"{baseUrl}/{productDetailPath}/{productSlug}";
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

        private static string GetCountryDisplayName(
            string? countryName,
            string? countryCode,
            string? coverageDescription)
        {
            if (!string.IsNullOrWhiteSpace(countryName)
                && !string.Equals(
                    countryName.Trim(),
                    countryCode?.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            {
                return countryName.Trim();
            }

            if (!string.IsNullOrWhiteSpace(coverageDescription))
                return coverageDescription.Trim();

            if (!string.IsNullOrWhiteSpace(countryName))
                return countryName.Trim();

            return countryCode?.Trim() ?? string.Empty;
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

            public string? CountryCode { get; set; }

            public string? CountryName { get; set; }

            public string? FlagUrl { get; set; }

            public string PackageName { get; set; } = string.Empty;

            public string ProviderPackageCode { get; set; } = string.Empty;

            public decimal? DataAmount { get; set; }

            public string? DataUnit { get; set; }

            public bool IsUnlimited { get; set; }

            public int ValidityDays { get; set; }

            public string? CoverageDescription { get; set; }

            public string? ActivationPolicy { get; set; }

            public string? SpeedPolicy { get; set; }

            public bool HotspotSupported { get; set; }

            public bool PhoneNumberSupported { get; set; }

            public bool SmsSupported { get; set; }
        }

        private sealed class PriceProjection
        {
            public Guid ProductId { get; set; }

            public Guid? ProductVariantId { get; set; }

            public decimal SalePrice { get; set; }

            public string? Currency { get; set; }
        }
    }
}
