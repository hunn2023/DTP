using Azure.Core;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Provider.Application.Abstractions;
using DTP.Modules.Provider.Application.Abstractions.Clients;
using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.Abstractions.Services;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Shared.Application;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ProvisionCountryDto = DTP.Modules.Catalog.Application.DTOs.ProvisionCountryDto;
using ProvisionProviderEsimProductResult = DTP.Modules.Catalog.Application.DTOs.ProvisionProviderEsimProductResult;

namespace DTP.Modules.Provider.Application.Services;

public sealed class ProviderPackageSyncService : IProviderPackageSyncService
{
    private const int PackagePageSize = 100;

    private readonly IProviderRepository _providerRepository;
    private readonly IProviderPackageProductRepository _packageRepository;
    private readonly IProviderProductMappingRepository _mappingRepository;
    private readonly IProviderApiLogRepository _apiLogRepository;
    private readonly IProviderUnitOfWork _unitOfWork;
    private readonly IPeacomProviderClient _peacomProviderClient;
    private readonly ICatalogProductProvisioningService _catalogProvisioningService;

    public ProviderPackageSyncService(
        IProviderRepository providerRepository,
        IProviderPackageProductRepository packageRepository,
        IProviderProductMappingRepository mappingRepository,
        IProviderApiLogRepository apiLogRepository,
        IProviderUnitOfWork unitOfWork,
        IPeacomProviderClient peacomProviderClient,
        ICatalogProductProvisioningService catalogProvisioningService)
    {
        _providerRepository = providerRepository;
        _packageRepository = packageRepository;
        _mappingRepository = mappingRepository;
        _apiLogRepository = apiLogRepository;
        _unitOfWork = unitOfWork;
        _peacomProviderClient = peacomProviderClient;
        _catalogProvisioningService = catalogProvisioningService;
    }

    public async Task<Result<ProviderPackageSyncResult>> SyncAsync(
        string providerCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(providerCode))
        {
            return Result<ProviderPackageSyncResult>.Failure(
                "ProviderCode không được rỗng.");
        }

        providerCode = providerCode.Trim();

        var provider = await _providerRepository.GetByCodeAsync(
            providerCode,
            cancellationToken);

        if (provider is null)
        {
            return Result<ProviderPackageSyncResult>.Failure(
                $"Provider '{providerCode}' không tồn tại.");
        }

        if (!provider.IsActive)
        {
            return Result<ProviderPackageSyncResult>.Failure(
                $"Provider '{providerCode}' đang inactive.");
        }

        var syncResult = new ProviderPackageSyncResult();

        /*
         * =========================================================
         * BƯỚC 1: GET PACKAGE PRODUCT
         * =========================================================
         *
         * Chỉ lấy danh sách package thô:
         * - ProviderSku
         * - ProviderProductId
         * - Tên provider
         * - Giá nhập
         * - Tồn kho
         *
         * Không tạo Product/ProductVariant ở bước này.
         */
        IReadOnlyList<ProviderPackageProductRemoteDto> remotePackages;

        try
        {
            remotePackages =
                await _peacomProviderClient.GetPackageProductsAsync(
                    provider,
                    PackagePageSize,
                    cancellationToken)
                ?? Array.Empty<ProviderPackageProductRemoteDto>();

            syncResult.Total = remotePackages.Count;

            await AddApiLogAsync(
                provider.Id,
                endpoint: "GET PACKAGE PRODUCT",
                response: $"Total: {remotePackages.Count}",
                statusCode: 200,
                isSuccess: true,
                errorMessage: null,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await AddApiErrorLogSafeAsync(
                provider.Id,
                "GET PACKAGE PRODUCT",
                ex,
                cancellationToken);

            return Result<ProviderPackageSyncResult>.Failure(
                $"Lỗi khi gọi GET PACKAGE PRODUCT: {ex.Message}");
        }

        if (remotePackages.Count == 0)
        {
            return Result<ProviderPackageSyncResult>.Success(syncResult);
        }

        /*
         * Loại package SKU rỗng và SKU trùng.
         */
        var distinctRemotePackages = remotePackages
            .Where(x =>
            {
                if (!string.IsNullOrWhiteSpace(x.Sku))
                    return true;

                syncResult.Failed++;
                syncResult.Errors.Add(
                    $"ProviderProductId={x.Id}: SKU bị rỗng.");

                return false;
            })
            .GroupBy(
                x => x.Sku.Trim(),
                StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Last())
            .ToList();

        /*
         * Dictionary giữ entity vừa upsert.
         * Vòng lấy detail không cần query lại database.
         */
        var syncedPackages =
            new Dictionary<string, ProviderPackageProduct>(
                StringComparer.OrdinalIgnoreCase);

        /*
         * =========================================================
         * BƯỚC 2: UPSERT PACKAGE THÔ
         * =========================================================
         */
        foreach (var remotePackage in distinctRemotePackages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var providerSku = remotePackage.Sku.Trim();

            try
            {
                var package = await UpsertProviderPackageAsync(
                    provider,
                    remotePackage,
                    cancellationToken);

                syncedPackages[providerSku] = package;
                syncResult.Synced++;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                syncResult.Failed++;

                syncResult.Errors.Add(
                    $"{providerSku}: Lưu package thô thất bại - {ex.Message}");
            }
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return Result<ProviderPackageSyncResult>.Failure(
                $"Không thể lưu package thô: {ex.Message}");
        }

        /*
         * =========================================================
         * BƯỚC 3: GET PRODUCT ESIM
         * =========================================================
         *
         * API này trả detail của cùng ProviderSku.
         *
         * Từ detail, DTP tự xác định:
         *
         * Product:
         * Provider + MainCountry + FamilyCode
         *
         * ProductVariant:
         * DataType + DataAmount + DataUnit + ValidityDays
         */
        foreach (var entry in syncedPackages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var providerSku = entry.Key;
            var package = entry.Value;

            try
            {
                await ProcessPackageDetailAsync(
                    provider,
                    package,
                    cancellationToken);

                syncResult.Provisioned++;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                syncResult.Failed++;

                syncResult.Errors.Add(
                    $"{providerSku}: Provision thất bại - {ex.Message}");

                await MarkPackageFailedSafeAsync(
                    package,
                    ex,
                    syncResult,
                    cancellationToken);
            }
        }

        return Result<ProviderPackageSyncResult>.Success(syncResult);
    }

    private async Task ProcessPackageDetailAsync(
        Domain.Entities.Provider provider,
        ProviderPackageProduct package,
        CancellationToken cancellationToken)
    {
        /*
         * Gọi API ngoài trước, không mở transaction database.
         */
        ProviderEsimProductRemoteDto detail;

        try
        {
            detail = await _peacomProviderClient.GetProductEsimAsync(
                provider,
                package.ProviderSku,
                cancellationToken);

            if (detail is null)
            {
                throw new InvalidOperationException(
                    "Provider không trả về dữ liệu chi tiết.");
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await AddApiErrorLogSafeAsync(
                provider.Id,
                $"GET PRODUCT ESIM - {package.ProviderSku}",
                ex,
                cancellationToken);

            throw;
        }

        ValidateDetailSku(package, detail);

        /*
         * Lưu RawJson trước.
         *
         * Nếu bước chuẩn hóa hoặc provision lỗi,
         * admin vẫn có dữ liệu provider để kiểm tra.
         */
        package.MarkDetailSynced(detail.RawJson);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        /*
         * Tạo request đã chuẩn hóa cho Catalog.
         */
        var provisionRequest = BuildProvisionRequest(
            provider,
            package,
            detail);

        /*
         * Catalog service phải đảm bảo idempotent:
         *
         * Product        tìm bằng ProductGroupingKey
         * ProductVariant tìm bằng ProductId + VariantKey
         */
        var provisionResult =
            await _catalogProvisioningService
                .ProvisionProviderEsimProductAsync(
                    provisionRequest,
                    cancellationToken);

        if (provisionResult is null)
        {
            throw new InvalidOperationException(
                "Catalog provisioning không trả về kết quả.");
        }

        await UpsertMappingAsync(
            provider,
            package,
            provisionResult,
            cancellationToken);

        package.MarkProvisioned();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<ProviderPackageProduct> UpsertProviderPackageAsync(
        Domain.Entities.Provider provider,
        ProviderPackageProductRemoteDto remotePackage,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(remotePackage.Sku))
        {
            throw new InvalidOperationException(
                "SKU của provider package không được rỗng.");
        }

        var providerSku = remotePackage.Sku.Trim();

        var existing =
            await _packageRepository.GetByProviderSkuAsync(
                provider.Id,
                providerSku,
                cancellationToken);

        if (existing is not null)
        {
            existing.UpdatePackageInfo(
                remotePackage.Id,
                remotePackage.Name,
                remotePackage.Model,
                remotePackage.Regional,
                null,
                remotePackage.Price,
                NormalizeCurrencyCode(remotePackage.CurrencyCode),
                remotePackage.ImageUrl,
                remotePackage.RawJson);

            return existing;
        }

        var package = new ProviderPackageProduct(
            provider.Id,
            providerSku,
            remotePackage.Id,
            remotePackage.Name,
            remotePackage.Model,
            remotePackage.Regional,
            null,
            remotePackage.Price,
            NormalizeCurrencyCode(remotePackage.CurrencyCode),
            remotePackage.ImageUrl,
            remotePackage.RawJson);

        await _packageRepository.AddAsync(
            package,
            cancellationToken);

        return package;
    }

    private async Task UpsertMappingAsync(
        Domain.Entities.Provider provider,
        ProviderPackageProduct package,
        ProvisionProviderEsimProductResult provisionResult,
        CancellationToken cancellationToken)
    {
        var existing =
            await _mappingRepository.GetByProviderSkuAsync(
                provider.Id,
                package.ProviderSku,
                cancellationToken);

        if (existing is null)
        {
            var mapping = new ProviderProductMapping(
                provider.Id,
                package.ProviderSku,
                package.ProviderProductId,
                provisionResult.ProductId,
                provisionResult.ProductVariantId,
                provisionResult.ProductPriceId,
                provisionResult.EsimPackageId);

            await _mappingRepository.AddAsync(
                mapping,
                cancellationToken);

            return;
        }

        existing.UpdateCatalogMapping(
            provisionResult.ProductId,
            provisionResult.ProductVariantId,
            provisionResult.ProductPriceId,
            provisionResult.EsimPackageId);
    }

    private static ProvisionProviderEsimProductRequest BuildProvisionRequest(
    Domain.Entities.Provider provider,
    ProviderPackageProduct package,
    ProviderEsimProductRemoteDto detail)
    {
        var mainCountry = ResolveMainCountry(
            package,
            detail);

        var familyCode = ResolveFamilyCode(
            package.ProviderSku,
            package.Model,
            detail.Slug,
            mainCountry.Code);

        var dataUnit = NormalizeDataUnit(
            detail.DataUnit);

        var variantName = BuildVariantName(

            detail.DataType,
            detail.IsUnlimited,
            detail.DataAmount ?? 0,
            dataUnit,
            detail.ValidityDays);

        /*
         * Family chỉ dùng ở VariantSku,
         * không dùng để chia Product.
         */
        var variantSku = BuildInternalVariantSku(
            mainCountry.Code,
            familyCode,
            detail.IsUnlimited,
            detail.DataAmount ?? 0,
            dataUnit,
            detail.ValidityDays);

        var price = package.Price > 0
            ? package.Price
            : detail.Price;

        return new ProvisionProviderEsimProductRequest
        {
            ProviderId = provider.Id,
            ProviderCode = provider.Code,
            ProviderSku = package.ProviderSku,
            ProviderProductId = package.ProviderProductId,

            ProductFamilyCode = familyCode,

            ProductName = $"eSIM {mainCountry.Name}",
            ProductDescription = detail.CoverageDescription,
            DataType = detail.DataType,
            VariantName = BuildVariantName(
                detail.DataType,
                 detail.IsUnlimited,
                detail.DataAmount ?? 0,
                dataUnit,
                detail.ValidityDays),

            VariantSku = BuildInternalVariantSku(
                mainCountry.Code,
                familyCode,
                detail.IsUnlimited,
                detail.DataAmount ?? 0,
                dataUnit,
                detail.ValidityDays),

            Price = package.Price > 0
                ? package.Price
                : detail.Price,

            CurrencyCode = string.IsNullOrWhiteSpace(package.CurrencyCode)
                ? "VND"
                : package.CurrencyCode.Trim().ToUpperInvariant(),

            DataAmount = detail.DataAmount ?? 0,
            DataUnit = dataUnit,
            ValidityDays = detail.ValidityDays,
            IsUnlimited = detail.IsUnlimited,

            CoverageType = detail.CoverageType,
            CoverageDescription = detail.CoverageDescription,

            Countries =
                [
                    new ProvisionCountryDto
                    {
                        CountryCode = mainCountry.Code,
                        CountryName = mainCountry.Name
                    }
                ],

            Operators = detail.Operators,
            IsActive = false
        };
    }


  




    private static string GenerateSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Guid.NewGuid().ToString("N");

        var slug = RemoveDiacritics(value)
            .Trim()
            .ToLowerInvariant();

        slug = Regex.Replace(
            slug,
            @"[^a-z0-9]+",
            "-");

        return slug.Trim('-');
    }

    private static string RemoveDiacritics(string value)
    {
        var normalized = value.Normalize(
            NormalizationForm.FormD);

        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(
                character);

            if (category != UnicodeCategory.NonSpacingMark)
                builder.Append(character);
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .Replace("đ", "d")
            .Replace("Đ", "D");
    }



    private static MainCountryInfo ResolveMainCountry(
        ProviderPackageProduct package,
        ProviderEsimProductRemoteDto detail)
    {
        /*
         * Ưu tiên quốc gia đầu tiên được client chuẩn hóa
         * từ GET PRODUCT ESIM.
         */
        var firstCountry = detail.Countries?
            .FirstOrDefault(x =>
                !string.IsNullOrWhiteSpace(x.Code));

        if (firstCountry is not null)
        {
            var code = NormalizeCountryCode(firstCountry.Code);

            if (!string.IsNullOrWhiteSpace(code))
            {
                var name = string.IsNullOrWhiteSpace(firstCountry.Name)
                    ? code
                    : firstCountry.Name.Trim();

                return new MainCountryInfo(code, name);
            }
        }

        /*
         * Fallback từ Regional.
         */
        var regionalCode = MapCountryNameToCode(
            package.Regional);

        if (!string.IsNullOrWhiteSpace(regionalCode))
        {
            return new MainCountryInfo(
                regionalCode,
                package.Regional?.Trim() ?? regionalCode);
        }

        /*
         * Fallback từ ProviderSku.
         *
         * Ví dụ:
         * BLC-01-ES-guay-mobile-30days-10gb
         */
        var skuCountryCode = ResolveCountryCodeFromSku(
            package.ProviderSku);

        if (!string.IsNullOrWhiteSpace(skuCountryCode))
        {
            return new MainCountryInfo(
                skuCountryCode,
                skuCountryCode);
        }

        throw new InvalidOperationException(
            $"Không xác định được quốc gia chính cho SKU " +
            $"'{package.ProviderSku}'.");
    }

    private static string ResolveFamilyCode(
        string providerSku,
        string? model,
        string? detailSlug,
        string mainCountryCode)
    {
        /*
         * Ưu tiên model nếu provider trả model.
         *
         * guay-mobile-30days-10gb → GUAY-MOBILE
         * DTAC219                 → DTAC219
         */
        var familyFromModel = RemoveVariantSuffix(model);

        if (!string.IsNullOrWhiteSpace(familyFromModel))
        {
            return NormalizeKeyPart(familyFromModel);
        }

        var value = providerSku.Trim();

        /*
         * Bỏ BLC-01-, BLC-03-...
         */
        value = Regex.Replace(
            value,
            @"^BLC-\d+-",
            string.Empty,
            RegexOptions.IgnoreCase);

        /*
         * Bỏ eSIM-
         */
        value = Regex.Replace(
            value,
            @"^eSIM-",
            string.Empty,
            RegexOptions.IgnoreCase);

        /*
         * Bỏ quốc gia đầu SKU.
         *
         * ES-guay-mobile-30days-10gb
         * → guay-mobile-30days-10gb
         */
        value = Regex.Replace(
            value,
            $"^{Regex.Escape(mainCountryCode)}-",
            string.Empty,
            RegexOptions.IgnoreCase);

        var patterns = new[]
        {
            /*
             * XMTYT5GB-30 → XMTY
             */
            @"^(?<family>.+?)T\d+(?:\.\d+)?(?:GB|MB|M)-\d+$",

            /*
             * XMTY500M-01 → XMTY
             */
            @"^(?<family>.+?)\d+(?:\.\d+)?(?:GB|MB|M)-\d+$",

            /*
             * guay-mobile-30days-10gb → guay-mobile
             */
            @"^(?<family>.+?)-\d+days?-\d+(?:\.\d+)?(?:gb|mb)$",

            /*
             * KRMAX-07 → KRMAX
             */
            @"^(?<family>.+?)-\d+$"
        };

        foreach (var pattern in patterns)
        {
            var match = Regex.Match(
                value,
                pattern,
                RegexOptions.IgnoreCase);

            if (!match.Success)
                continue;

            var family = match.Groups["family"].Value;

            if (!string.IsNullOrWhiteSpace(family))
                return NormalizeKeyPart(family);
        }

        /*
         * Slug chỉ là fallback.
         */
        if (!string.IsNullOrWhiteSpace(detailSlug))
            return NormalizeKeyPart(detailSlug);

        /*
         * Không parse được thì dùng SKU đã bỏ prefix.
         *
         * Cách này tránh gộp nhầm Product.
         */
        return NormalizeKeyPart(value);
    }

    private static string? RemoveVariantSuffix(string? model)
    {
        if (string.IsNullOrWhiteSpace(model))
            return null;

        var value = model.Trim();

        var result = Regex.Replace(
            value,
            @"-\d+days?-\d+(?:\.\d+)?(?:gb|mb)$",
            string.Empty,
            RegexOptions.IgnoreCase);

        /*
         * Nếu model có format duration/data thì trả family.
         */
        if (!string.Equals(
                value,
                result,
                StringComparison.OrdinalIgnoreCase))
        {
            return result;
        }

        /*
         * Model như DTAC219 được xem là family.
         */
        return value;
    }




    private static string BuildVariantName(
    int dataType,
    bool isUnlimited,
    decimal dataAmount,
    string? dataUnit,
    int validityDays)
    {
        if (isUnlimited)
            return $"Không giới hạn dung lượng - {validityDays} ngày";

        var amountText = FormatDataAmount(
            dataAmount,
            dataUnit);

        return dataType switch
        {
            1 => $"Tổng {amountText} - {validityDays} ngày",

            2 => $"{amountText}/ngày - {validityDays} ngày",

            _ => $"{amountText} - {validityDays} ngày"
        };
    }

    private static string BuildInternalVariantSku(
    string countryCode,
    string familyCode,
    bool isUnlimited,
    decimal dataAmount,
    string dataUnit,
    int validityDays)
    {
        var dataType = isUnlimited
            ? "D"
            : "T";

        var amount = dataAmount
            .ToString(
                "0.##",
                CultureInfo.InvariantCulture)
            .Replace(".", "_");

        return string.Join(
            "-",
            "DTP",
            NormalizeKeyPart(countryCode),
            NormalizeKeyPart(familyCode),
            $"{dataType}{amount}{NormalizeKeyPart(dataUnit)}",
            $"{validityDays}D");
    }


    private static string FormatDataAmount(
    decimal dataAmount,
    string? dataUnit)
    {
        var normalizedUnit = NormalizeDataUnit(dataUnit);

        if (normalizedUnit == "MB" &&
            dataAmount >= 1024 &&
            dataAmount % 1024 == 0)
        {
            return $"{dataAmount / 1024m:0.##}GB";
        }

        return $"{dataAmount:0.##}{normalizedUnit}";
    }

    private static string NormalizeDataUnit(string? dataUnit)
    {
        if (string.IsNullOrWhiteSpace(dataUnit))
            return "MB";

        return dataUnit.Trim().ToUpperInvariant() switch
        {
            "M" => "MB",
            "MEGABYTE" => "MB",
            "MEGABYTES" => "MB",

            "G" => "GB",
            "GIGABYTE" => "GB",
            "GIGABYTES" => "GB",

            var value => value
        };
    }

    private static string NormalizeCurrencyCode(
        string? currencyCode)
    {
        if (string.IsNullOrWhiteSpace(currencyCode))
            return "VND";

        return currencyCode.Trim().ToUpperInvariant();
    }

    private static string NormalizeCountryCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value.Trim().ToUpperInvariant() switch
        {
            "SGP" => "SG",
            "MYS" => "MY",
            "IDN" => "ID",
            "THA" => "TH",
            "KOR" => "KR",
            "VNM" => "VN",
            "JPN" => "JP",
            "CHN" => "CN",
            "USA" => "US",

            var code when code.Length == 2 => code,

            _ => string.Empty
        };
    }

    private static string MapCountryNameToCode(
        string? countryName)
    {
        if (string.IsNullOrWhiteSpace(countryName))
            return string.Empty;

        return countryName.Trim().ToUpperInvariant() switch
        {
            "SINGAPORE" => "SG",
            "MALAYSIA" => "MY",
            "INDONESIA" => "ID",
            "THAILAND" => "TH",

            "SOUTH KOREA" => "KR",
            "SOUTH KOREA（SKT）" => "KR",
            "SOUTH KOREA (SKT)" => "KR",

            "VIETNAM" => "VN",
            "VIET NAM" => "VN",

            "JAPAN" => "JP",
            "CHINA" => "CN",
            "SPAIN" => "ES",
            "FRANCE" => "FR",
            "UNITED STATES" => "US",

            _ => string.Empty
        };
    }

    private static string ResolveCountryCodeFromSku(
        string? providerSku)
    {
        if (string.IsNullOrWhiteSpace(providerSku))
            return string.Empty;

        /*
         * Không dùng IgnoreCase.
         *
         * Tránh BLC-03-eSIM bị hiểu nhầm "eS" là country code.
         */
        var match = Regex.Match(
            providerSku,
            @"^BLC-\d+-(?<country>[A-Z]{2})-");

        if (!match.Success)
            return string.Empty;

        return NormalizeCountryCode(
            match.Groups["country"].Value);
    }

    private static string NormalizeKeyPart(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "UNKNOWN";

        return Regex.Replace(
                value.Trim().ToUpperInvariant(),
                @"[^A-Z0-9]+",
                "-")
            .Trim('-');
    }

    private static void ValidateDetailSku(
        ProviderPackageProduct package,
        ProviderEsimProductRemoteDto detail)
    {
        if (string.IsNullOrWhiteSpace(detail.Sku))
        {
            throw new InvalidOperationException(
                "GET PRODUCT ESIM trả về SKU rỗng.");
        }
        if (!detail.IsUnlimited &&
                 detail.DataAmount <= 0)
        {
            throw new ArgumentException(
                "DataAmount phải lớn hơn 0 nếu không phải unlimited.");
        }


        if (!string.Equals(
                package.ProviderSku.Trim(),
                detail.Sku.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"SKU không khớp. " +
                $"PackageSku={package.ProviderSku}, " +
                $"DetailSku={detail.Sku}.");
        }
    }

    private async Task AddApiLogAsync(
        Guid providerId,
        string endpoint,
        string? response,
        int? statusCode,
        bool isSuccess,
        string? errorMessage,
        CancellationToken cancellationToken)
    {
        await _apiLogRepository.AddAsync(
            new ProviderApiLog(
                providerId,
                endpoint,
                "GET",
                null,
                response,
                statusCode,
                isSuccess,
                errorMessage),
            cancellationToken);
    }

    private async Task AddApiErrorLogSafeAsync(
        Guid providerId,
        string endpoint,
        Exception exception,
        CancellationToken cancellationToken)
    {
        try
        {
            await AddApiLogAsync(
                providerId,
                endpoint,
                response: null,
                statusCode: null,
                isSuccess: false,
                errorMessage: TruncateError(exception.Message),
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            /*
             * Không để lỗi ghi log che mất lỗi ban đầu.
             */
        }
    }

    private async Task MarkPackageFailedSafeAsync(
        ProviderPackageProduct package,
        Exception exception,
        ProviderPackageSyncResult syncResult,
        CancellationToken cancellationToken)
    {
        try
        {
            package.MarkFailed(
                TruncateError(exception.Message));

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception markFailedException)
        {
            syncResult.Errors.Add(
                $"{package.ProviderSku}: Không thể cập nhật Failed - " +
                markFailedException.Message);
        }
    }

    private static string TruncateError(
        string? error,
        int maximumLength = 2000)
    {
        if (string.IsNullOrWhiteSpace(error))
            return "Unknown error.";

        var value = error.Trim();

        return value.Length <= maximumLength
            ? value
            : value[..maximumLength];
    }

    private sealed record MainCountryInfo(
        string Code,
        string Name);
}