using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;
using StackExchange.Redis;


namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class EsimPackageService : IEsimPackageService
    {
        private readonly IEsimPackageRepository _esimPackageRepository;
        private readonly ICacheService _cacheService;

        public EsimPackageService(
            IEsimPackageRepository esimPackageRepository,
            ICacheService cacheService)
        {
            _esimPackageRepository = esimPackageRepository;
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
            {
                return Result<PagedResultDto<EsimPackageDto>>.Success(cachedData);
            }

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
            slug = slug.Trim().ToLower();

            var cacheKey = EsimPackageCacheKeys.PublicBySlug(slug);

            var cachedData = await _cacheService.GetAsync<EsimPackageDto>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<EsimPackageDto?>.Success(cachedData);
            }

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

            var ezsim = await _esimPackageRepository.GetPagedAsync(
                keyword,
                productVariantId,
                countryId,
                carrierId,
                isActive,
                pageIndex,
                pageSize,
                cancellationToken);

            return Result<PagedResultDto<EsimPackageDto>>.Success(ezsim);
        }

        public async Task<Result<EsimPackageDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _esimPackageRepository.GetByIdDtoAsync(
                id,
                cancellationToken);

            return Result<EsimPackageDto?>.Success(result);
        }

        public async Task<Result<Guid>> CreateAsync(
            CreateEsimPackageCommand command,
            CancellationToken cancellationToken = default)
        {
            var id = await _esimPackageRepository.CreateAsync(
                command,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                EsimPackageCacheKeys.PublicPrefix,
                cancellationToken);

            return Result<Guid>.Success(id);
        }

        public async Task<Result> UpdateAsync(
            UpdateEsimPackageCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _esimPackageRepository.UpdateAsync(
                command,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                EsimPackageCacheKeys.PublicPrefix,
                cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _esimPackageRepository.DeleteAsync(
                id,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                EsimPackageCacheKeys.PublicPrefix,
                cancellationToken);

            return Result.Success();
        }
    }
}