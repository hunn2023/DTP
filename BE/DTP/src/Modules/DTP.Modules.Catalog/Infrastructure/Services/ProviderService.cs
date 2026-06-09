using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProviderService : IProviderService
    {
        private readonly IProviderRepository _providerRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public ProviderService(
            IProviderRepository providerRepository,
            ICatalogUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _providerRepository = providerRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<Result<List<ProviderDto>>> GetActiveAsync(
            CancellationToken cancellationToken = default)
        {
            var cacheKey = ProviderCacheKeys.ActiveList;

            var cachedData = await _cacheService.GetAsync<List<ProviderDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData != null)
                return Result<List<ProviderDto>>.Success(cachedData);


            var providers = await _providerRepository.GetListAsync(
                keyword: null,
                cancellationToken);

            var result = providers
                .Where(x => x.IsActive)
                .OrderBy(x => x.Id)
                .ThenBy(x => x.Name)
                .Select(x => new ProviderDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    //Slug = x.Slug,
                    //LogoUrl = x.LogoUrl,
                    //Website = x.Website,
                    //SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .ToList();

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(12),
                cancellationToken);

            return Result<List<ProviderDto>>.Success(result);

        }

        public async Task<Result<ProviderDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = ProviderCacheKeys.Detail(id);

            var cachedData = await _cacheService.GetAsync<ProviderDto>(
                cacheKey,
                cancellationToken);

            if (cachedData != null)
                return Result<ProviderDto?>.Success(cachedData);

            var provider = await _providerRepository.GetByIdAsync(id, cancellationToken);

            if (provider == null)
                return Result<ProviderDto?>.Success(null);

            var result = new ProviderDto
            {
                Id = provider.Id,
                Code = provider.Code,
                Name = provider.Name,
                //Slug = provider.Slug,
                //LogoUrl = provider.LogoUrl,
                //Website = provider.Website,
                //SortOrder = provider.SortOrder,
                IsActive = provider.IsActive
            };

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(12),
                cancellationToken);

            return Result<ProviderDto?>.Success(result);
        }

        public async Task ClearProviderCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                ProviderCacheKeys.Prefix,
                cancellationToken);
        }

        public async Task<Result<PagedResultDto<ProviderDto>>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var result = await _providerRepository.GetPagedAsync(
                keyword,
                pageIndex,
                pageSize,
                cancellationToken);

            return Result<PagedResultDto<ProviderDto>>.Success(result);
        }
    }
}