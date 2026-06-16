using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;

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

        public async Task<ProvisionProviderEsimProductResult> ProvisionProviderEsimProductAsync(
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
                    mainCountry.Id,
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
                    request,
                    esimPackage.Id,
                    cancellationToken);

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
                throw;
            }
            catch (Exception ex)
            {
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

            product.Activate();
            variant.Activate();
            price?.Activate();
            esimPackage.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static void ValidateRequest(ProvisionProviderEsimProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ProviderCode))
                throw new ArgumentException("ProviderCode không được rỗng.");

            if (string.IsNullOrWhiteSpace(request.ProviderSku))
                throw new ArgumentException("ProviderSku không được rỗng.");

            if (string.IsNullOrWhiteSpace(request.ProductName))
                throw new ArgumentException("Tên sản phẩm không được rỗng.");

            if (string.IsNullOrWhiteSpace(request.VariantName))
                throw new ArgumentException("Tên variant không được rỗng.");

            if (request.ValidityDays <= 0)
                throw new ArgumentException("ValidityDays phải lớn hơn 0.");

            if (!request.IsUnlimited && request.DataAmount is null)
                throw new ArgumentException("DataAmount không được rỗng nếu không phải unlimited.");

            if (request.Price < 0)
                throw new ArgumentException("Giá không hợp lệ.");

            if (request.Countries.Count == 0)
                throw new ArgumentException("Coverage country không được rỗng.");
        }

        private async Task<Country> EnsureMainCountryAsync(
            ProvisionProviderEsimProductRequest request,
            CancellationToken cancellationToken)
        {
            var firstCountry = request.Countries.First();

            var countryCode = firstCountry.CountryCode.Trim().ToUpperInvariant();

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

            await _countryRepository.AddAsync(country, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return country;
        }

        private async Task<Product> EnsureProductAsync(
            ProvisionProviderEsimProductRequest request,
            Guid countryId,
            CancellationToken cancellationToken)
        {
            var productCode = BuildProductCode(request);

            var category = await _categoryRepository.GetByCodeAsync(
                "ESIM",
                cancellationToken);


            if (category is null)
            {
                category = new Category(
                    code: "ESIM",
                    name: "eSIM",
                    slug: "esim",
                     null,
                    sortOrder: 1,
                    isActive: false);
                await _categoryRepository.AddAsync(category, cancellationToken);
            }

            var existing = await _productRepository.GetByCodeAsync(
                productCode,
                cancellationToken);

            if (existing is not null)
            {
                existing.UpdateBasicInfo(
                    request.ProductName,
                    GenerateSlug(request.ProductName),
                    request.ProductDescription,
                    isActive: false);

                return existing;
            }

            var product = new Product(
                code: productCode,
                name: request.ProductName,
                slug: GenerateSlug(request.ProductName),
                categoryId: category.Id,
                countryId: countryId,
                shortDescription: request.ProductDescription,
                description: request.ProductDescription,
                locationText: request.CoverageDescription,
                thumbnailUrl: null,
                isFeatured: false,
                isHot: false,
                sortOrder: 0,
                isActive: false);

            await _productRepository.AddAsync(product, cancellationToken);

            return product;
        }

        private async Task<ProductVariant> EnsureProductVariantAsync(
            ProvisionProviderEsimProductRequest request,
            Guid productId,
            CancellationToken cancellationToken)
        {
            var sku = request.VariantSku ?? request.ProviderSku;

            var existing = await _productVariantRepository.GetBySkuAsync(
                sku,
                cancellationToken);

            if (existing is not null)
            {
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

            await _productVariantRepository.AddAsync(variant, cancellationToken);

            return variant;
        }

        private async Task<ProductPrice> EnsureProductPriceAsync(
            ProvisionProviderEsimProductRequest request,
            Guid productId,
            Guid productVariantId,
            CancellationToken cancellationToken)
        {
            var existing = await _productPriceRepository.GetActiveByProductVariantAsync(
                productId,
                productVariantId,
                request.CurrencyCode,
                cancellationToken);

            if (existing is not null)
            {
                existing.Update(
                    originalPrice: request.Price,
                    salePrice: request.Price,
                    costPrice: 0,
                    startDate: null,
                    endDate: null,
                    isActive: false);

                return existing;
            }

            var price = new ProductPrice(
                productId: productId,
                productVariantId: productVariantId,
                currency: request.CurrencyCode,
                originalPrice: request.Price,
                salePrice: request.Price,
                costPrice: 0,
                startDate: null,
                endDate: null,
                null,
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
            var providerPackageCode = request.ProviderSku;

            var existing = await _esimPackageRepository.GetByProviderPackageCodeAsync̣(
                providerPackageCode,
                cancellationToken);

            if (existing is not null)
            {
                existing.Update(
                    name: request.VariantName,
                    slug: GenerateSlug($"{request.ProductName}-{request.VariantName}"),
                    dataAmount: request.DataAmount,
                    dataUnit: request.DataUnit ?? "GB",
                    validityDays: request.ValidityDays,
                    isUnlimited: request.IsUnlimited,
                    coverageType: request.CoverageType ?? "SingleCountry",
                    coverageDescription: request.CoverageDescription,
                    activationPolicy: "ActivateWhenInstalled",
                    speedPolicy: null,
                    hotspotSupported: true,
                    phoneNumberSupported: false,
                    smsSupported: false,
                    kycRequired: false,
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
                name: request.VariantName,
                slug: GenerateSlug($"{request.ProductName}-{request.VariantName}"),
                providerPackageCode: providerPackageCode,
                dataAmount: request.DataAmount,
                dataUnit: request.DataUnit ?? "GB",
                validityDays: request.ValidityDays,
                isUnlimited: request.IsUnlimited,
                coverageType: request.CoverageType ?? "SingleCountry",
                coverageDescription: request.CoverageDescription,
                activationPolicy: "ActivateWhenInstalled",
                speedPolicy: null,
                hotspotSupported: true,
                phoneNumberSupported: false,
                smsSupported: false,
                kycRequired: false,
                qrDeliveryType: "Email",
                sortOrder: 0,
                isActive: false);

            await _esimPackageRepository.AddAsync(esimPackage, cancellationToken);

            return esimPackage;
        }

        private async Task EnsureCoveragesAsync(
            ProvisionProviderEsimProductRequest request,
            Guid esimPackageId,
            CancellationToken cancellationToken)
        {
            await _esimPackageCoverageRepository.DeleteByEsimPackageIdAsync(
                esimPackageId,
                cancellationToken);

            foreach (var countryDto in request.Countries)
            {
                var countryCode = countryDto.CountryCode.Trim().ToUpperInvariant();

                var country = await _countryRepository.GetByCodeAsync(
                    countryCode,
                    cancellationToken);

                if (country is null)
                {
                    country = new Country(
                        countryCode,
                        countryDto.CountryName,
                        GenerateSlug(countryDto.CountryName),
                        flagUrl: null,
                        null,
                        null,
                        sortOrder: 0,
                        isActive: false);

                    await _countryRepository.AddAsync(country, cancellationToken);
                }

                var coverage = new EsimPackageCoverage(
                    esimPackageId,
                    country.Id,
                    countryCode,
                    countryDto.CountryName);

                await _esimPackageCoverageRepository.AddAsync(
                    coverage,
                    cancellationToken);
            }
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
    }
}