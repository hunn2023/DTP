
using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
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
        public async Task<Result<PagedResultDto<CategoryDto>>> GetPublicAsync(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var cacheKey = CategoryCacheKeys.PublicActivePaged(pageIndex, pageSize);

            var cachedData = await _cacheService.GetAsync<PagedResultDto<CategoryDto>>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return Result<PagedResultDto<CategoryDto>>.Success(cachedData);
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

            return Result<PagedResultDto<CategoryDto>>.Success(result);
        }

        public async Task<Result<CategoryDto?>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = CategoryCacheKeys.Detail(id);

            var cachedData = await _cacheService.GetAsync<CategoryDto>(
                cacheKey,
                cancellationToken);

            if (cachedData != null)
                return Result<CategoryDto?>.Success(cachedData);

            var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);

            if (category == null)
                return Result<CategoryDto?>.Success(null);

            var result = new CategoryDto
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                Slug = category.Slug,
                SortOrder = category.SortOrder,
                IsActive = category.IsActive
            };

            await _cacheService.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromHours(6),
                cancellationToken);

            return Result<CategoryDto?>.Success(result);
        }

        public async Task ClearCategoryCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                CategoryCacheKeys.Prefix,
                cancellationToken);
        }

        public async Task<Result<CategoryDto>> CreateAsync(
                   string? code,
                   string name,
                   string slug,
                   int sortOrder,
                   CancellationToken cancellationToken = default)
        {

            name = name?.Trim() ?? string.Empty;
            slug = slug?.Trim() ?? string.Empty;
            code = code?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return Result<CategoryDto>.Failure("Tên danh mục không được để trống.");

            if (string.IsNullOrWhiteSpace(slug))
                return Result<CategoryDto>.Failure("Slug không được để trống.");

            var existsName = await _categoryRepository.ExistsByNameAsync(
                name,
                null,
                cancellationToken);

            if (existsName)
                return Result<CategoryDto>.Failure("Tên danh mục đã tồn tại.");

            var existsSlug = await _categoryRepository.ExistsBySlugAsync(
                slug,
                null,
                cancellationToken);

            if (existsSlug)
                return Result<CategoryDto>.Failure("Slug danh mục đã tồn tại.");

            if (!string.IsNullOrWhiteSpace(code))
            {
                var existsCode = await _categoryRepository.ExistsByCodeAsync(
                    code,
                    null,
                    cancellationToken);

                if (existsCode)
                    return Result<CategoryDto>.Failure("Mã danh mục đã tồn tại.");
            }

            var category = new Category(
                code,
                name,
                slug,
                null,
                sortOrder);

            await _categoryRepository.AddAsync(category, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearCategoryCacheAsync(cancellationToken);

            var dto = new CategoryDto
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                Slug = category.Slug,
                SortOrder = category.SortOrder
            };

            return Result<CategoryDto>.Success(dto);
        }


        public async Task<Result> DeleteAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            var category = await _categoryRepository.GetByIdAsync(Id, cancellationToken);

            if (category == null)
                return Result.Failure("Không tìm thấy danh mục.");

            _categoryRepository.Remove(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
            await ClearCategoryCacheAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result<CategoryDto>> UpdateAsync(
                Guid id,
                string? code,
                string name,
                string slug,
                int sortOrder,
                CancellationToken cancellationToken = default)
        {
            name = name?.Trim() ?? string.Empty;
            slug = slug?.Trim() ?? string.Empty;
            code = code?.Trim();

            if (id == Guid.Empty)
                return Result<CategoryDto>.Failure("Id danh mục không hợp lệ.");

            if (string.IsNullOrWhiteSpace(name))
                return Result<CategoryDto>.Failure("Tên danh mục không được để trống.");

            if (string.IsNullOrWhiteSpace(slug))
                return Result<CategoryDto>.Failure("Slug không được để trống.");

            var category = await _categoryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (category == null)
                return Result<CategoryDto>.Failure("Danh mục không tồn tại.");

            var existsName = await _categoryRepository.ExistsByNameAsync(
                name,
                id,
                cancellationToken);

            if (existsName)
                return Result<CategoryDto>.Failure("Tên danh mục đã tồn tại.");

            var existsSlug = await _categoryRepository.ExistsBySlugAsync(
                slug,
                id,
                cancellationToken);

            if (existsSlug)
                return Result<CategoryDto>.Failure("Slug danh mục đã tồn tại.");

            if (!string.IsNullOrWhiteSpace(code))
            {
                var existsCode = await _categoryRepository.ExistsByCodeAsync(
                    code,
                    id,
                    cancellationToken);

                if (existsCode)
                    return Result<CategoryDto>.Failure("Mã danh mục đã tồn tại.");
            }

            category.Update(
                code,
                name,
                slug,
                null,
                sortOrder);

            _categoryRepository.Update(category);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await ClearCategoryCacheAsync(cancellationToken);

            var dto = new CategoryDto
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                Slug = category.Slug,
                IsActive = category.IsActive,
                SortOrder = category.SortOrder
            };

            return Result<CategoryDto>.Success(dto);
        }


    }
}
