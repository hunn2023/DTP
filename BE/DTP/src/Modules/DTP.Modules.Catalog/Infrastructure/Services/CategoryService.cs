using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICatalogUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CategoryService(
            ICategoryRepository categoryRepository,
            ICatalogUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }
        public async Task<PagedResultDto<CategoryDto>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var cacheKey = CategoryCacheKeys.PublicActivePaged(
                pageIndex,
                pageSize);

            var cachedData = await _cacheService.GetAsync<PagedResultDto<CategoryDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return cachedData;
            }

            var result = await _categoryRepository.GetPublicPagedAsync(
                pageIndex,
                pageSize,
                cancellationToken);

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(6),
                cancellationToken);

            return result;
        }

        public async Task<CategoryDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = CategoryCacheKeys.Detail(id);

            var cachedData = await _cacheService.GetAsync<CategoryDto>(
                cacheKey,
                cancellationToken);

            if (cachedData != null)
                return cachedData;

            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

            if (category == null)
                return null;

            var result = new CategoryDto
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                Slug = category.Slug,
                //ParentId = category.ParentId,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive
            };

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(6),
                cancellationToken);

            return result;
        }

        public async Task ClearCategoryCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                CategoryCacheKeys.Prefix,
                cancellationToken);
        }
    }
}
