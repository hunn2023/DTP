using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class EsimPackageService : IEsimPackageService
    {
        private readonly IEsimPackageRepository _esimPackageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public EsimPackageService(
            IEsimPackageRepository esimPackageRepository,
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            ICountryRepository countryRepository,
            ICatalogUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _esimPackageRepository = esimPackageRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _countryRepository = countryRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<PagedResultDto<EsimPackageDto>>> GetPublicPagedAsync(
            Guid? countryId,
            Guid? carrierId,
            bool? isUnlimited,
            int? validityDays,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var cacheKey = EsimPackageCacheKeys.PublicPaged(
                countryId,
                carrierId,
                isUnlimited,
                validityDays,
                pageIndex,
                pageSize);

            var cachedData = await _cacheService.GetAsync<PagedResultDto<EsimPackageDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
                return Result<PagedResultDto<EsimPackageDto>>.Success(cachedData);

            var result = await _esimPackageRepository.GetPublicPagedAsync(
                countryId,
                carrierId,
                isUnlimited,
                validityDays,
                pageIndex,
                pageSize,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(1),
                cancellationToken);

            return Result<PagedResultDto<EsimPackageDto>>.Success(result);
        }

        public async Task<Result<EsimPackageDto?>> GetPublicBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return Result<EsimPackageDto?>.Failure("Slug không hợp lệ.");

            slug = slug.Trim().ToLower();

            var cacheKey = EsimPackageCacheKeys.PublicBySlug(slug);

            var cachedData = await _cacheService.GetAsync<EsimPackageDto>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
                return Result<EsimPackageDto?>.Success(cachedData);

            var result = await _esimPackageRepository.GetPublicBySlugAsync(
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

            return Result<EsimPackageDto?>.Success(result);
        }

        public async Task<Result<PagedResultDto<EsimPackageDto>>> GetPagedAsync(
            string? keyword,
            Guid? productVariantId,
            Guid? countryId,
            Guid? carrierId,
            bool? isActive,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var result = await _esimPackageRepository.GetPagedAsync(
                keyword,
                productVariantId,
                countryId,
                carrierId,
                isActive,
                pageIndex,
                pageSize,
                cancellationToken);

            return Result<PagedResultDto<EsimPackageDto>>.Success(result);
        }

        public async Task<Result<EsimPackageDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result<EsimPackageDto?>.Failure("Id không hợp lệ.");

            var result = await _esimPackageRepository.GetByIdDtoAsync(
                id,
                cancellationToken);

            return Result<EsimPackageDto?>.Success(result);
        }

        public async Task<Result<Guid>> CreateAsync(
            CreateEsimPackageCommand command,
            CancellationToken cancellationToken = default)
        {
            var validationResult = await ValidateCreateAsync(command, cancellationToken);

            if (!validationResult.IsSuccess)
                return Result<Guid>.Failure(validationResult.Error);

            var slug = command.Slug.Trim().ToLower();

            var entity = new EsimPackage(
                command.ProductId,
                command.ProductVariantId,
                command.ProviderId,
                command.CountryId,
                command.Name.Trim(),
                slug,
                command.ProviderPackageCode.Trim(),
                command.IsUnlimited ? null : command.DataAmount,
                command.DataUnit.Trim(),
                command.ValidityDays,
                command.IsUnlimited,
                command.CoverageType.Trim(),
                command.CoverageDescription?.Trim(),
                command.ActivationPolicy.Trim(),
                command.SpeedPolicy?.Trim(),
                command.HotspotSupported,
                command.PhoneNumberSupported,
                command.SmsSupported,
                command.KycRequired,
                command.QrDeliveryType.Trim(),
                command.SortOrder,
                command.IsActive);

            await _esimPackageRepository.AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearPublicCacheAsync(cancellationToken);

            return Result<Guid>.Success(entity.Id);
        }
        public async Task<Result> UpdateAsync(
            UpdateEsimPackageCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.Id == Guid.Empty)
                return Result.Failure("Id không hợp lệ.");

            var entity = await _esimPackageRepository.GetByIdAsync(
                command.Id,
                cancellationToken);

            if (entity is null)
                return Result.Failure("Không tìm thấy gói eSIM.");

            var validationResult = await ValidateUpdateAsync(command, cancellationToken);

            if (!validationResult.IsSuccess)
                return Result.Failure(validationResult.Error);

            var slug = command.Slug.Trim().ToLower();

            entity.Update(
                command.ProductId,
                command.ProductVariantId,
                command.ProviderId,
                command.CountryId,
                command.Name.Trim(),
                slug,
                command.ProviderPackageCode.Trim(),
                command.IsUnlimited ? null : command.DataAmount,
                command.DataUnit.Trim(),
                command.ValidityDays,
                command.IsUnlimited,
                command.CoverageType.Trim(),
                command.CoverageDescription?.Trim(),
                command.ActivationPolicy.Trim(),
                command.SpeedPolicy?.Trim(),
                command.HotspotSupported,
                command.PhoneNumberSupported,
                command.SmsSupported,
                command.KycRequired,
                command.QrDeliveryType.Trim(),
                command.SortOrder,
                command.IsActive);

            _esimPackageRepository.Update(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearPublicCacheAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id không hợp lệ.");

            var entity = await _esimPackageRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (entity is null)
                return Result.Failure("Không tìm thấy gói eSIM.");

            _esimPackageRepository.Remove(entity);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearPublicCacheAsync(cancellationToken);

            return Result.Success();
        }

        private async Task<Result> ValidateCreateAsync(
            CreateEsimPackageCommand command,
            CancellationToken cancellationToken)
        {
            if (command.ProductId == Guid.Empty)
                return Result.Failure("ProductId không hợp lệ.");

            if (command.ProductVariantId == Guid.Empty)
                return Result.Failure("ProductVariantId không hợp lệ.");

            if (command.ProviderId == Guid.Empty)
                return Result.Failure("ProviderId không hợp lệ.");

            if (command.CountryId == Guid.Empty)
                return Result.Failure("CountryId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Failure("Vui lòng nhập tên gói eSIM.");

            if (string.IsNullOrWhiteSpace(command.Slug))
                return Result.Failure("Vui lòng nhập slug.");

            if (string.IsNullOrWhiteSpace(command.ProviderPackageCode))
                return Result.Failure("Vui lòng nhập mã gói từ provider.");

            if (string.IsNullOrWhiteSpace(command.DataUnit))
                return Result.Failure("Vui lòng nhập đơn vị dung lượng.");

            if (!command.IsUnlimited && (!command.DataAmount.HasValue || command.DataAmount <= 0))
                return Result.Failure("Gói giới hạn dung lượng cần có DataAmount lớn hơn 0.");

            if (command.ValidityDays <= 0)
                return Result.Failure("Số ngày sử dụng phải lớn hơn 0.");

            if (string.IsNullOrWhiteSpace(command.CoverageType))
                return Result.Failure("Vui lòng nhập loại vùng phủ sóng.");

            if (string.IsNullOrWhiteSpace(command.ActivationPolicy))
                return Result.Failure("Vui lòng nhập chính sách kích hoạt.");

            if (string.IsNullOrWhiteSpace(command.QrDeliveryType))
                return Result.Failure("Vui lòng nhập hình thức giao QR.");

            var productExists = await _productRepository.ExistsAsync(
                command.ProductId,
                cancellationToken);

            if (!productExists)
                return Result.Failure("Không tìm thấy sản phẩm.");

            var variant = await _productVariantRepository.GetByIdAsync(
                command.ProductVariantId,
                cancellationToken);

            if (variant is null)
                return Result.Failure("Không tìm thấy biến thể sản phẩm.");

            if (variant.ProductId != command.ProductId)
                return Result.Failure("Biến thể sản phẩm không thuộc sản phẩm đã chọn.");



            var countryExists = await _countryRepository.ExistsAsync(
                command.CountryId,
                cancellationToken);

            if (!countryExists)
                return Result.Failure("Không tìm thấy quốc gia.");

            var slug = command.Slug.Trim().ToLower();

            var slugExists = await _esimPackageRepository.ExistsSlugAsync(
                slug,
                null,
                cancellationToken);

            if (slugExists)
                return Result.Failure("Slug gói eSIM đã tồn tại.");

            var providerPackageCodeExists = await _esimPackageRepository.ExistsProviderPackageCodeAsync(
                command.ProviderId,
                command.ProviderPackageCode.Trim(),
                null,
                cancellationToken);

            if (providerPackageCodeExists)
                return Result.Failure("Mã gói provider đã tồn tại.");

            return Result.Success();
        }

        private async Task<Result> ValidateUpdateAsync(
            UpdateEsimPackageCommand command,
            CancellationToken cancellationToken)
        {
            if (command.ProductId == Guid.Empty)
                return Result.Failure("ProductId không hợp lệ.");

            if (command.ProductVariantId == Guid.Empty)
                return Result.Failure("ProductVariantId không hợp lệ.");

            if (command.ProviderId == Guid.Empty)
                return Result.Failure("ProviderId không hợp lệ.");

            if (command.CountryId == Guid.Empty)
                return Result.Failure("CountryId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Failure("Vui lòng nhập tên gói eSIM.");

            if (string.IsNullOrWhiteSpace(command.Slug))
                return Result.Failure("Vui lòng nhập slug.");

            if (string.IsNullOrWhiteSpace(command.ProviderPackageCode))
                return Result.Failure("Vui lòng nhập mã gói từ provider.");

            if (string.IsNullOrWhiteSpace(command.DataUnit))
                return Result.Failure("Vui lòng nhập đơn vị dung lượng.");

            if (!command.IsUnlimited && (!command.DataAmount.HasValue || command.DataAmount <= 0))
                return Result.Failure("Gói không giới hạn cần có dung lượng lớn hơn 0.");

            if (command.ValidityDays <= 0)
                return Result.Failure("Số ngày sử dụng phải lớn hơn 0.");

            if (string.IsNullOrWhiteSpace(command.CoverageType))
                return Result.Failure("Vui lòng nhập loại vùng phủ sóng.");

            if (string.IsNullOrWhiteSpace(command.ActivationPolicy))
                return Result.Failure("Vui lòng nhập chính sách kích hoạt.");

            if (string.IsNullOrWhiteSpace(command.QrDeliveryType))
                return Result.Failure("Vui lòng nhập hình thức giao QR.");

            var productExists = await _productRepository.ExistsAsync(
                command.ProductId,
                cancellationToken);

            if (!productExists)
                return Result.Failure("Không tìm thấy sản phẩm.");

            var variant = await _productVariantRepository.GetByIdAsync(
                command.ProductVariantId,
                cancellationToken);

            if (variant is null)
                return Result.Failure("Không tìm thấy biến thể sản phẩm.");

            if (variant.ProductId != command.ProductId)
                return Result.Failure("Biến thể sản phẩm không thuộc sản phẩm đã chọn.");


           var countryExists = await _countryRepository.ExistsAsync(
                command.CountryId,
                cancellationToken);

            if (!countryExists)
                return Result.Failure("Không tìm thấy quốc gia.");

            var slug = command.Slug.Trim().ToLower();

            var slugExists = await _esimPackageRepository.ExistsSlugAsync(
                slug,
                command.Id,
                cancellationToken);

            if (slugExists)
                return Result.Failure("Slug gói eSIM đã tồn tại.");

            var providerPackageCodeExists = await _esimPackageRepository.ExistsProviderPackageCodeAsync(
                command.ProviderId,
                command.ProviderPackageCode.Trim(),
                command.Id,
                cancellationToken);

            if (providerPackageCodeExists)
                return Result.Failure("Mã gói provider đã tồn tại.");

            return Result.Success();
        }

        private async Task ClearPublicCacheAsync(
            CancellationToken cancellationToken)
        {
            await _cacheService.RemoveByPrefixAsync(
                EsimPackageCacheKeys.PublicPrefix,
                cancellationToken);
        }
    }
}