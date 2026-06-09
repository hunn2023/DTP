using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.Commands.Products;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Infrastructure.Repositories;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductPriceRepository _productPriceRepository;
        private readonly IEsimPackageRepository _esimPackageRepository;
        private readonly ICacheService _cacheService;
        private const int MaxPageSize = 100;


        public ProductService(
          IProductRepository productRepository,
          IProductPriceRepository productPriceRepository,
          IEsimPackageRepository esimPackageRepository,
          ICacheService cacheService)
        {
            _productRepository = productRepository;
            _productPriceRepository = productPriceRepository;
            _esimPackageRepository = esimPackageRepository;
            _cacheService = cacheService;
        }

        public async Task<Result<ProductDetailDto?>> GetDetailAsync(
         Guid id,
         CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result<ProductDetailDto?>.Failure("Id sản phẩm không hợp lệ.");


            var product = await _productRepository.GetDetailAsync(
                id,
                cancellationToken);

            if (product == null)
                return Result<ProductDetailDto?>.Failure("Không tìm thấy sản phẩm.");


            var prices = await _productPriceRepository.GetByProductIdAsync(
                id,
                cancellationToken);

            var esimPackages = await _esimPackageRepository.GetByProductIdAsync(
                id,
                cancellationToken);

            var result = new ProductDetailDto
            {
                Id = product.Id,
                Code = product.Code,
                Name = product.Name,
                Slug = product.Slug,

                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,

                CountryId = product.CountryId,
                CountryName = product.Country?.Name,

                ShortDescription = product.ShortDescription,
                Description = product.Description,
                LocationText = product.LocationText,
                ThumbnailUrl = product.ThumbnailUrl,

                IsFeatured = product.IsFeatured,
                IsHot = product.IsHot,
                SoldCount = product.SoldCount,

                SortOrder = product.SortOrder,
                IsActive = product.IsActive,

                Images = product.Images
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new ProductImageDto
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ImageUrl = x.ImageUrl,
                        //ImageKey = x.ImageKey,
                        AltText = x.AltText,
                        SortOrder = x.SortOrder,
                        IsThumbnail = x.IsThumbnail,
                        ContentType = x.ContentType,
                        Size = x.Size,
                        IsActive = x.IsActive
                    })
                    .ToList(),

                Variants = product.Variants
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new ProductVariantDto
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        Sku = x.Sku,
                        Name = x.Name,
                        ShortName = x.ShortName,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive
                    })
                    .ToList(),

                Attributes = product.Attributes
                    .OrderBy(x => x.SortOrder)
                    .Select(x => new ProductAttributeDto
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        Key = x.Key,
                        Value = x.Value,
                        SortOrder = x.SortOrder
                    })
                    .ToList(),

                Prices = prices
                    .Select(x => new ProductPriceDto
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductVariantId = x.ProductVariantId,
                        Currency = x.Currency,
                        OriginalPrice = x.OriginalPrice,
                        SalePrice = x.SalePrice,
                        CostPrice = x.CostPrice,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        IsActive = x.IsActive
                    })
                    .ToList(),

                EsimPackages = esimPackages
                    .Select(x => new EsimPackageDto
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        ProductVariantId = x.ProductVariantId,

                        ProviderId = x.ProviderId,
                        ProviderName = x.Provider != null ? x.Provider.Name : null,

                        CountryId = x.CountryId,
                        CountryName = x.Country != null ? x.Country.Name : null,

                        Name = x.Name,
                        Slug = x.Slug,
                        ProviderPackageCode = x.ProviderPackageCode,

                        DataAmount = x.DataAmount,
                        DataUnit = x.DataUnit,
                        ValidityDays = x.ValidityDays,
                        IsUnlimited = x.IsUnlimited,

                        CoverageType = x.CoverageType,
                        CoverageDescription = x.CoverageDescription,
                        ActivationPolicy = x.ActivationPolicy,
                        SpeedPolicy = x.SpeedPolicy,

                        HotspotSupported = x.HotspotSupported,
                        PhoneNumberSupported = x.PhoneNumberSupported,
                        SmsSupported = x.SmsSupported,
                        KycRequired = x.KycRequired,

                        QrDeliveryType = x.QrDeliveryType,
                        SortOrder = x.SortOrder,
                        IsActive = x.IsActive
                    })
                    .ToList()
            };

            return Result<ProductDetailDto?>.Success(result);

        }



        public async Task<Result<PagedResultDto<ProductDto>>> GetPublicPagedAsync(
            string? keyword,
            Guid? categoryId,
            Guid? countryId,
            Guid? carrierId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            var cacheKey = ProductCacheKeys.PublicPaged(
                keyword,
                categoryId,
                countryId,
                carrierId,
                pageIndex,
                pageSize);

            var cachedData = await _cacheService.GetAsync<PagedResultDto<ProductDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<PagedResultDto<ProductDto>>.Success(cachedData);
            }

            var result = await _productRepository.GetPublicPagedAsync(
                keyword,
                categoryId,
                countryId,
                pageIndex,
                pageSize,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(1),
                cancellationToken);

            return Result<PagedResultDto<ProductDto>>.Success(result);
        }

        public async Task<Result<ProductDto?>> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Result<ProductDto?>.Failure("Slug sản phẩm không được để trống.");

            slug = slug.Trim().ToLower();

            var cacheKey = ProductCacheKeys.PublicBySlug(slug);

            var cachedData = await _cacheService.GetAsync<ProductDto>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<ProductDto?>.Success(cachedData);
            }

            var result = await _productRepository.GetPublicBySlugAsync(
                slug,
                cancellationToken);

            if (result is not null)
            {
                await _cacheService.SetAsync(
                    cacheKey,
                    result,
                    TimeSpan.FromHours(1),
                    cancellationToken);
            }

            return Result<ProductDto?>.Success(result);
        }

        public async Task<Result<PagedResultDto<ProductDto>>> GetPagedAsync(
            string? keyword,
            Guid? categoryId,
            Guid? countryId,
            Guid? carrierId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            NormalizePaging(ref pageIndex, ref pageSize);

            var result = await _productRepository.GetPagedAsync(
                keyword,
                categoryId,
                countryId,
                isActive,
                pageIndex,
                pageSize,
                cancellationToken);

            return Result<PagedResultDto<ProductDto>>.Success(result);
        }

        public async Task<Result<ProductDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result<ProductDto?>.Failure("Id sản phẩm không hợp lệ.");

            var result = await _productRepository.GetByIdDtoAsync(
                id,
                cancellationToken);

            if (result is null)
                return Result<ProductDto?>.Failure("Không tìm thấy sản phẩm.");

            return Result<ProductDto?>.Success(result);
        }

        public async Task<Result<Guid>> CreateAsync(
            CreateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var validateResult = ValidateCreate(command);

            if (!validateResult.IsSuccess)
                return Result<Guid>.Failure(validateResult.Error);

            NormalizeCreateCommand(command);

            var productId = await _productRepository.CreateAsync(
                command,
                cancellationToken);

            await ClearProductCacheAsync(cancellationToken);

            return Result<Guid>.Success(productId);
        }

        public async Task<Result> UpdateAsync(
            UpdateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var validateResult = ValidateUpdate(command);

            if (!validateResult.IsSuccess)
                return Result.Failure(validateResult.Error);

            NormalizeUpdateCommand(command);

            var result = await _productRepository.UpdateAsync(
                command,
                cancellationToken);

            if (!result)
                return Result.Failure("Không tìm thấy sản phẩm để cập nhật.");

            await ClearProductCacheAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id sản phẩm không hợp lệ.");

            var result = await _productRepository.DeleteAsync(
                id,
                cancellationToken);

            if (!result)
                return Result.Failure("Không tìm thấy sản phẩm để xoá.");

            await ClearProductCacheAsync(cancellationToken);

            return Result.Success();
        }

        private static Result ValidateCreate(CreateProductCommand command)
        {
            if (command is null)
                return Result.Failure("Dữ liệu sản phẩm không hợp lệ.");

            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Failure("Tên sản phẩm không được để trống.");

            if (command.Name.Trim().Length > 250)
                return Result.Failure("Tên sản phẩm không được vượt quá 250 ký tự.");

            if (string.IsNullOrWhiteSpace(command.Slug))
                return Result.Failure("Slug sản phẩm không được để trống.");

            if (command.Slug.Trim().Length > 250)
                return Result.Failure("Slug sản phẩm không được vượt quá 250 ký tự.");

            if (command.CategoryId == Guid.Empty)
                return Result.Failure("Vui lòng chọn danh mục sản phẩm.");

            if (command.SortOrder < 0)
                return Result.Failure("Thứ tự hiển thị không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(command.Code) && command.Code.Trim().Length > 100)
                return Result.Failure("Mã sản phẩm không được vượt quá 100 ký tự.");

            if (!string.IsNullOrWhiteSpace(command.ShortDescription) && command.ShortDescription.Trim().Length > 500)
                return Result.Failure("Mô tả ngắn không được vượt quá 500 ký tự.");

            if (!string.IsNullOrWhiteSpace(command.LocationText) && command.LocationText.Trim().Length > 250)
                return Result.Failure("Thông tin vị trí không được vượt quá 250 ký tự.");

            if (!string.IsNullOrWhiteSpace(command.ThumbnailUrl) && command.ThumbnailUrl.Trim().Length > 1000)
                return Result.Failure("Đường dẫn ảnh đại diện không được vượt quá 1000 ký tự.");

            return Result.Success();
        }

        private static Result ValidateUpdate(UpdateProductCommand command)
        {
            if (command is null)
                return Result.Failure("Dữ liệu sản phẩm không hợp lệ.");

            if (command.Id == Guid.Empty)
                return Result.Failure("Id sản phẩm không hợp lệ.");

            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Failure("Tên sản phẩm không được để trống.");

            if (command.Name.Trim().Length > 250)
                return Result.Failure("Tên sản phẩm không được vượt quá 250 ký tự.");

            if (string.IsNullOrWhiteSpace(command.Slug))
                return Result.Failure("Slug sản phẩm không được để trống.");

            if (command.Slug.Trim().Length > 250)
                return Result.Failure("Slug sản phẩm không được vượt quá 250 ký tự.");

            if (command.CategoryId == Guid.Empty)
                return Result.Failure("Vui lòng chọn danh mục sản phẩm.");

            if (command.SortOrder < 0)
                return Result.Failure("Thứ tự hiển thị không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(command.Code) && command.Code.Trim().Length > 100)
                return Result.Failure("Mã sản phẩm không được vượt quá 100 ký tự.");

            if (!string.IsNullOrWhiteSpace(command.ShortDescription) && command.ShortDescription.Trim().Length > 500)
                return Result.Failure("Mô tả ngắn không được vượt quá 500 ký tự.");

            if (!string.IsNullOrWhiteSpace(command.LocationText) && command.LocationText.Trim().Length > 250)
                return Result.Failure("Thông tin vị trí không được vượt quá 250 ký tự.");

            if (!string.IsNullOrWhiteSpace(command.ThumbnailUrl) && command.ThumbnailUrl.Trim().Length > 1000)
                return Result.Failure("Đường dẫn ảnh đại diện không được vượt quá 1000 ký tự.");

            return Result.Success();
        }

        private static void NormalizeCreateCommand(CreateProductCommand command)
        {
            command.Code = command.Code?.Trim();
            command.Name = command.Name.Trim();
            command.Slug = command.Slug.Trim().ToLower();
            command.ShortDescription = command.ShortDescription?.Trim();
            command.Description = command.Description?.Trim();
            command.LocationText = command.LocationText?.Trim();
            command.ThumbnailUrl = command.ThumbnailUrl?.Trim();
        }

        private static void NormalizeUpdateCommand(UpdateProductCommand command)
        {
            command.Code = command.Code?.Trim();
            command.Name = command.Name.Trim();
            command.Slug = command.Slug.Trim().ToLower();
            command.ShortDescription = command.ShortDescription?.Trim();
            command.Description = command.Description?.Trim();
            command.LocationText = command.LocationText?.Trim();
            command.ThumbnailUrl = command.ThumbnailUrl?.Trim();
        }

        private static void NormalizePaging(ref int pageIndex, ref int pageSize)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            if (pageSize > MaxPageSize)
                pageSize = MaxPageSize;
        }

        private async Task ClearProductCacheAsync(CancellationToken cancellationToken)
        {
            await _cacheService.RemoveByPrefixAsync(
                "catalog:products:public",
                cancellationToken);
        }
    }
}