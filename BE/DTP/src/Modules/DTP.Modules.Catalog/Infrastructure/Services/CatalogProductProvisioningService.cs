using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.RegularExpressions;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class CatalogProductProvisioningService : ICatalogProductProvisioningService
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IEsimPackageRepository _esimPackageRepository;
        private readonly IEsimPackageCoverageRepository _esimPackageCoverageRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly ICategoryRepository _categoryRepository;

        public CatalogProductProvisioningService(
            ICountryRepository countryRepository,
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            IProductPriceRepository productPriceRepository,
            IEsimPackageRepository esimPackageRepository,
            IEsimPackageCoverageRepository esimPackageCoverageRepository,
            ICatalogUnitOfWork unitOfWork,
            ICategoryRepository categoryRepository)
        {
            _countryRepository = countryRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _productPriceRepository = productPriceRepository;
            _esimPackageRepository = esimPackageRepository;
            _esimPackageCoverageRepository = esimPackageCoverageRepository;
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProvisionProviderEsimProductResult>
    ProvisionProviderEsimProductAsync(
        ProvisionProviderEsimProductRequest request,
        CancellationToken cancellationToken = default)
        {
            try
            {
                ValidateRequest(request);

                var mainCountry = await EnsureMainCountryAsync(
                    request,
                    cancellationToken);

                var product = await EnsureProductAsync(
                    request,
                    mainCountry,
                    cancellationToken);

                var variant = await EnsureProductVariantAsync(
                    request,
                    product.Id,
                    cancellationToken);

                var price = await EnsureProductPriceAsync(
                    request,
                    product.Id,
                    variant.Id,
                    cancellationToken);

                var esimPackage = await EnsureEsimPackageAsync(
                    request,
                    product.Id,
                    variant.Id,
                    mainCountry.Id,
                    cancellationToken);

                await EnsureCoveragesAsync(
                    mainCountry,
                    esimPackage.Id,
                    cancellationToken);

                /*
                 * Chỉ SaveChanges một lần.
                 *
                 * EF Core tự bọc một SaveChanges trong transaction.
                 */
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ProvisionProviderEsimProductResult
                {
                    ProductId = product.Id,
                    ProductVariantId = variant.Id,
                    ProductPriceId = price.Id,
                    EsimPackageId = esimPackage.Id
                };
            }
            catch (OperationCanceledException)
            {
                _unitOfWork.ClearTracking();
                throw;
            }
            catch (Exception ex)
            {
                /*
                 * Nếu không clear, Product/ProductVariant/EsimPackage có trạng thái
                 * Added sẽ tiếp tục bị insert lại khi xử lý SKU sau.
                 */
                _unitOfWork.ClearTracking();

                throw new InvalidOperationException(
                    BuildProvisionExceptionMessage(request, ex),
                    ex);
            }
        }


        private static string BuildProvisionExceptionMessage(
            ProvisionProviderEsimProductRequest request,
            Exception ex)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Provision sản phẩm eSIM từ provider thất bại.");
            sb.AppendLine($"ProviderCode: {request?.ProviderCode}");
            sb.AppendLine($"SKU: {request?.ProviderSku}");
            sb.AppendLine($"ProductName: {request?.ProductName}");
            sb.AppendLine($"ExceptionType: {ex.GetType().FullName}");
            sb.AppendLine($"Message: {ex.Message}");

            if (ex is DbUpdateException dbEx)
            {
                sb.AppendLine("DbUpdateException:");

                foreach (var entry in dbEx.Entries)
                {
                    sb.AppendLine($"- Entity: {entry.Entity.GetType().Name}");
                    sb.AppendLine($"  State: {entry.State}");
                }
            }

            var inner = ex.InnerException;
            var level = 1;

            while (inner != null)
            {
                sb.AppendLine($"InnerException Level {level}:");
                sb.AppendLine($"Type: {inner.GetType().FullName}");
                sb.AppendLine($"Message: {inner.Message}");

                if (inner is SqlException sqlEx)
                {
                    sb.AppendLine("SqlException Details:");
                    sb.AppendLine($"Number: {sqlEx.Number}");
                    sb.AppendLine($"State: {sqlEx.State}");
                    sb.AppendLine($"Class: {sqlEx.Class}");
                    sb.AppendLine($"Server: {sqlEx.Server}");
                    sb.AppendLine($"Procedure: {sqlEx.Procedure}");
                    sb.AppendLine($"LineNumber: {sqlEx.LineNumber}");

                    foreach (SqlError error in sqlEx.Errors)
                    {
                        sb.AppendLine($"SQL Error:");
                        sb.AppendLine($"  Number: {error.Number}");
                        sb.AppendLine($"  State: {error.State}");
                        sb.AppendLine($"  Class: {error.Class}");
                        sb.AppendLine($"  Message: {error.Message}");
                        sb.AppendLine($"  Procedure: {error.Procedure}");
                        sb.AppendLine($"  LineNumber: {error.LineNumber}");
                    }
                }

                inner = inner.InnerException;
                level++;
            }

            return sb.ToString();
        }


        public async Task ActivateProviderProvisionedProductAsync(
            Guid productId,
            Guid productVariantId,
            Guid? productPriceId,
            Guid esimPackageId,
            CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(
                productId,
                cancellationToken);

            if (product is null)
                throw new InvalidOperationException("Product không tồn tại.");


            var variant = await _productVariantRepository.GetByIdAsync(
                productVariantId,
                cancellationToken);

            if (variant is null)
                throw new InvalidOperationException("ProductVariant không tồn tại.");

            ProductPrice? price = null;

            if (productPriceId.HasValue)
            {
                price = await _productPriceRepository.GetByIdAsync(
                    productPriceId.Value,
                    cancellationToken);

                if (price is null)
                    throw new InvalidOperationException("ProductPrice không tồn tại.");
            }

            var esimPackage = await _esimPackageRepository.GetByIdAsync(
                esimPackageId,
                cancellationToken);

            if (esimPackage is null)
                throw new InvalidOperationException("EsimPackage không tồn tại.");

            var country = await _countryRepository.GetByIdAsync(
                esimPackage.CountryId,
                cancellationToken);

            if (country is not null)
            {
                country.Activate();
            }

            product.Activate();
            variant.Activate();
            price?.Activate();
            esimPackage.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }




        private static void ValidateRequest(
    ProvisionProviderEsimProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.ProviderId == Guid.Empty)
                throw new ArgumentException("ProviderId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.ProviderCode))
                throw new ArgumentException("ProviderCode không được rỗng.");

            if (string.IsNullOrWhiteSpace(request.ProviderSku))
                throw new ArgumentException("ProviderSku không được rỗng.");

            if (string.IsNullOrWhiteSpace(request.ProductFamilyCode))
            {
                throw new ArgumentException(
                    "ProductFamilyCode không được rỗng.");
            }

            if (string.IsNullOrWhiteSpace(request.ProductName))
                throw new ArgumentException("Tên sản phẩm không được rỗng.");

            if (string.IsNullOrWhiteSpace(request.VariantName))
                throw new ArgumentException("Tên variant không được rỗng.");

            if (string.IsNullOrWhiteSpace(request.VariantSku))
                throw new ArgumentException("VariantSku không được rỗng.");

            if (request.ValidityDays <= 0)
                throw new ArgumentException("ValidityDays phải lớn hơn 0.");

            if (!request.IsUnlimited && request.DataAmount <= 0)
            {
                throw new ArgumentException(
                    "DataAmount phải lớn hơn 0 nếu không phải unlimited.");
            }

            if (request.Price <= 0)
                throw new ArgumentException("Giá phải lớn hơn 0.");

            if (request.Countries is null ||
                request.Countries.Count == 0)
            {
                throw new ArgumentException(
                    "Quốc gia chính không được rỗng.");
            }

            var firstCountry = request.Countries.First();

            if (string.IsNullOrWhiteSpace(firstCountry.CountryCode))
            {
                throw new ArgumentException(
                    "CountryCode chính không được rỗng.");
            }
        }


        private async Task<Country> EnsureMainCountryAsync(
    ProvisionProviderEsimProductRequest request,
    CancellationToken cancellationToken)
        {
            var firstCountry = request.Countries.First();

            var countryCode = firstCountry.CountryCode
                .Trim()
                .ToUpperInvariant();

            var country = await _countryRepository.GetByCodeAsync(
                countryCode,
                cancellationToken);

            if (country is not null)
                return country;

            country = new Country(
                countryCode,
                firstCountry.CountryName,
                GenerateSlug(firstCountry.CountryName),
                flagUrl: null,
                null,
                null,
                sortOrder: 0,
                isActive: false);

            await _countryRepository.AddAsync(
                country,
                cancellationToken);

            /*
             * Không SaveChanges ở đây.
             * Save cùng Product/Variant/Package ở cuối.
             */
            return country;
        }

        private async Task<Product> EnsureProductAsync(
    ProvisionProviderEsimProductRequest request,
    Country mainCountry,
    CancellationToken cancellationToken)
        {
            /*
             * Một quốc gia chỉ có một Product.
             */
            var productCode = BuildProductCode(
                mainCountry.Code);

            var productSlug = BuildProductSlug(
                mainCountry.Code);

            var productName = $"eSIM {mainCountry.Name}";

            var category = await _categoryRepository.GetByCodeAsync(
                "ESIM",
                cancellationToken);

            if (category is null)
            {
                category = new Category(
                    code: "ESIM",
                    name: "eSIM",
                    slug: "esim",
                    parentId: null,
                    sortOrder: 1,
                    isActive: false);

                await _categoryRepository.AddAsync(
                    category,
                    cancellationToken);
            }

            var existing = await _productRepository.GetByCodeAsync(
                productCode,
                cancellationToken);

            if (existing is not null)
            {
                existing.UpdateBasicInfo(
                    name: productName,
                    slug: productSlug,
                    description: request.ProductDescription,
                    isActive: false);

                return existing;
            }

            var product = new Product(
                code: productCode,
                name: productName,
                slug: productSlug,
                categoryId: category.Id,
                countryId: mainCountry.Id,
                shortDescription: "",
                description: request.ProductDescription,
                locationText: mainCountry.Name,
                thumbnailUrl: null,
                isFeatured: true,
                isHot: true,
                sortOrder: 0,
                isActive: false);

            await _productRepository.AddAsync(
                product,
                cancellationToken);

            return product;
        }

        private async Task<ProductVariant> EnsureProductVariantAsync(
    ProvisionProviderEsimProductRequest request,
    Guid productId,
    CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.VariantSku))
            {
                throw new InvalidOperationException(
                    "VariantSku nội bộ không được rỗng.");
            }

            var sku = request.VariantSku.Trim().ToUpperInvariant();

            var existing = await _productVariantRepository.GetBySkuAsync(
                sku,
                cancellationToken);

            if (existing is not null)
            {
                if (existing.ProductId != productId)
                {
                    throw new InvalidOperationException(
                        $"VariantSku '{sku}' đã thuộc Product khác.");
                }

                existing.Update(
                    request.VariantName,
                    shortName: request.VariantName,
                    description: request.ProductDescription,
                    sortOrder: 0,
                    isActive: false);

                return existing;
            }

            var variant = new ProductVariant(
                productId: productId,
                sku: sku,
                name: request.VariantName,
                shortName: request.VariantName,
                description: request.ProductDescription,
                sortOrder: 0,
                isActive: false);

            await _productVariantRepository.AddAsync(
                variant,
                cancellationToken);

            return variant;
        }

        private static decimal CalculateSalePrice(decimal costPrice)
        {
            const decimal priceMultiplier = 1.768m;

            var calculatedPrice = costPrice * priceMultiplier;

            return Math.Round(
                calculatedPrice / 1000m,
                0,
                MidpointRounding.AwayFromZero) * 1000m;
        }

        private async Task<ProductPrice> EnsureProductPriceAsync(
            ProvisionProviderEsimProductRequest request,
            Guid productId,
            Guid productVariantId,
            CancellationToken cancellationToken)
        {

            var costPrice = request.Price;
            var salePrice = CalculateSalePrice(costPrice);

            var existing = await _productPriceRepository.GetActiveByProductVariantAsync(
                productId,
                productVariantId,
                request.CurrencyCode,
                cancellationToken);

            if (existing is not null)
            {
                existing.Update(
                    originalPrice: request.Price,
                    salePrice: salePrice,
                    costPrice: costPrice,
                    startDate: DateTime.Now,
                    endDate: null,
                    isActive: false);

                return existing;
            }

            var price = new ProductPrice(
                productId: productId,
                productVariantId: productVariantId,
                currency: request.CurrencyCode,
                originalPrice: request.Price,
                salePrice: salePrice,
                costPrice: costPrice,
                startDate: DateTime.Now,
                endDate: null,
                "",
                isActive: false);

            await _productPriceRepository.AddAsync(price, cancellationToken);

            return price;
        }

        private async Task<EsimPackage> EnsureEsimPackageAsync(
    ProvisionProviderEsimProductRequest request,
    Guid productId,
    Guid productVariantId,
    Guid countryId,
    CancellationToken cancellationToken)
        {
            var providerPackageCode = request.ProviderSku.Trim();

            /*
             * Mỗi ProviderSku có một slug package riêng.
             *
             * BLUECOM + BLC-03-eSIM-XMTYT5GB-15
             * → bluecom-blc-03-esim-xmtyt5gb-15
             */
            var esimPackageSlug = BuildEsimPackageSlug(
                request.ProviderCode,
                providerPackageCode);

            var esimPackageName = BuildEsimPackageName(request);

            var existing =
                await _esimPackageRepository
                    .GetByProviderPackageCodeAsync(
                        request.ProviderId,
                        providerPackageCode,
                        cancellationToken);

            if (existing is not null)
            {
                existing.Update(
                    name: esimPackageName,

                    /*
                     * Không dùng request.Slug.
                     * request.Slug là slug cấp Product.
                     */
                    slug: esimPackageSlug,

                    dataAmount: request.DataAmount,
                    dataUnit: NormalizeDataUnit(request.DataUnit),
                    validityDays: request.ValidityDays,
                    isUnlimited: request.IsUnlimited,
                    coverageType:
                        request.CoverageType ?? "SingleCountry",
                    coverageDescription:
                        request.CoverageDescription,
                    activationPolicy: "ActivateWhenInstalled",
                    speedPolicy: "5G",
                    hotspotSupported: true,
                    phoneNumberSupported: true,
                    smsSupported: true,
                    kycRequired: true,
                    qrDeliveryType: "Email",
                    sortOrder: 0,
                    isActive: false);

                return existing;
            }

            var esimPackage = new EsimPackage(
                productId: productId,
                productVariantId: productVariantId,
                providerId: request.ProviderId,
                countryId: countryId,
                name: esimPackageName,

                /*
                 * Slug package riêng biệt.
                 */
                slug: esimPackageSlug,

                providerPackageCode: providerPackageCode,
                dataAmount: request.DataAmount,
                dataUnit: NormalizeDataUnit(request.DataUnit),
                validityDays: request.ValidityDays,
                isUnlimited: request.IsUnlimited,
                coverageType:
                    request.CoverageType ?? "SingleCountry",
                coverageDescription:
                    request.CoverageDescription,
                activationPolicy: "ActivateWhenInstalled",
                speedPolicy: "5G",
                hotspotSupported: true,
                phoneNumberSupported: true,
                smsSupported: true,
                kycRequired: true,
                qrDeliveryType: "Email",
                sortOrder: 0,
                isActive: false);

            await _esimPackageRepository.AddAsync(
                esimPackage,
                cancellationToken);

            return esimPackage;
        }

        private static string BuildEsimPackageName(
          ProvisionProviderEsimProductRequest request)
        {
            var countryName = request.Countries
               .First()
               .CountryName
               .Trim();

            if (request.IsUnlimited)
            {
                return $"eSIM {countryName} - Không giới hạn dung lượng " +
                       $"trong {request.ValidityDays} ngày";
            }

            var amountText = FormatDataAmount(
                request.DataAmount,
                request.DataUnit);

            return request.DataType switch
            {
                1 =>
                    $"eSIM {countryName} - Tổng {amountText} " +
                    $"sử dụng trong {request.ValidityDays} ngày",

                2 =>
                    $"eSIM {countryName} - {amountText}/ngày " +
                    $"trong {request.ValidityDays} ngày",

                _ =>
                    $"eSIM {countryName} - {amountText} " +
                    $"trong {request.ValidityDays} ngày"
            };
        }

        private static string FormatDataPackageName(
                int dataType,
                decimal dataAmount,
                string? dataUnit,
                int validityDays)
        {

            if(dataUnit == null)
            {
                return $"Không giới hạn trong {validityDays} ngày";
            }
            var amountText = FormatDataAmount(
                dataAmount,
                NormalizeDataUnit(dataUnit));

            return dataType switch
            {
                // Tổng dung lượng dùng trong toàn thời hạn
                1 => $"Tổng {amountText} sử dụng trong {validityDays} ngày",

                // Dung lượng reset mỗi ngày
                2 => $"{amountText}/ngày trong {validityDays} ngày",

                _ => $"{amountText} sử dụng trong {validityDays} ngày"
            };
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

        private async Task EnsureCoveragesAsync(
            Country country,
            Guid esimPackageId,
            CancellationToken cancellationToken)
        {
            await _esimPackageCoverageRepository.DeleteByEsimPackageIdAsync(
                esimPackageId,
                cancellationToken);

            var coverage = new EsimPackageCoverage(
                   esimPackageId,
                   country.Id,
                   country.Code,
                   country.Name);

            await _esimPackageCoverageRepository.AddAsync(
                coverage,
                cancellationToken);

            //foreach (var countryDto in request.Countries)
            //{
            //    var countryCode = countryDto.CountryCode.Trim().ToUpperInvariant();

            //    var country = await _countryRepository.GetByCodeAsync(
            //        countryCode,
            //        cancellationToken);

            //    if (country is null)
            //    {
            //        country = new Country(
            //            countryCode,
            //            countryDto.CountryName,
            //            GenerateSlug(countryDto.CountryName),
            //            flagUrl: null,
            //            null,
            //            null,
            //            sortOrder: 0,
            //            isActive: false);

            //        await _countryRepository.AddAsync(country, cancellationToken);
            //    }

            //    var coverage = new EsimPackageCoverage(
            //        esimPackageId,
            //        country.Id,
            //        countryCode,
            //        countryDto.CountryName);

            //    await _esimPackageCoverageRepository.AddAsync(
            //        coverage,
            //        cancellationToken);
            //}
        }

        private static string BuildProductCode(ProvisionProviderEsimProductRequest request)
        {
            return $"{request.ProviderCode}-{request.ProviderSku}"
                .Trim()
                .ToUpperInvariant();
        }

        private static string GenerateSlug(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Guid.NewGuid().ToString("N");

            var slug = value.Trim().ToLowerInvariant();

            slug = slug
                .Replace("đ", "d")
                .Replace(" ", "-")
                .Replace("_", "-")
                .Replace("/", "-")
                .Replace(".", "-");

            while (slug.Contains("--"))
            {
                slug = slug.Replace("--", "-");
            }

            return slug.Trim('-');
        }

        private static string BuildProductCode(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                throw new InvalidOperationException(
                    "CountryCode không được rỗng.");
            }

            return $"ESIM-{countryCode.Trim().ToUpperInvariant()}";
        }




        private static string BuildProductSlug(string countryCode)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
            {
                throw new InvalidOperationException(
                    "CountryCode không được rỗng.");
            }

            return GenerateSlug(
                $"esim-{countryCode.Trim().ToLowerInvariant()}");
        }

        private static string NormalizeCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException("Code không được rỗng.");

            var normalized = Regex.Replace(
                value.Trim().ToUpperInvariant(),
                @"[^A-Z0-9]+",
                "-");

            return normalized.Trim('-');
        }

        private static string BuildEsimPackageSlug(
    string providerCode,
    string providerSku)
        {
            if (string.IsNullOrWhiteSpace(providerCode))
            {
                throw new InvalidOperationException(
                    "ProviderCode không được rỗng.");
            }

            if (string.IsNullOrWhiteSpace(providerSku))
            {
                throw new InvalidOperationException(
                    "ProviderSku không được rỗng.");
            }

            return GenerateSlug(
                $"{providerCode}-{providerSku}");
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
    }
}