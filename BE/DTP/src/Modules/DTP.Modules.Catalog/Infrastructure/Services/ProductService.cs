using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Modules.Catalog.Application.Commands.Products;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;

        public ProductService(
            IProductRepository productRepository,
            ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
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
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

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
            slug = slug.Trim().ToLower();

            var cacheKey = ProductCacheKeys.PublicBySlug(slug);

            var cachedData = await _cacheService.GetAsync<ProductDto>(
                cacheKey,
                cancellationToken);

            if (cachedData is not null)
            {
                return cachedData;
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

            return result;
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
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var result = await _productRepository.GetPagedAsync(
                keyword,
                categoryId,
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
            var result = await _productRepository.GetByIdDtoAsync(
                id,
                cancellationToken);

            return Result<ProductDto?>.Success(result); 
        }

        public async Task<Result<Guid>> CreateAsync(
            CreateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var productId = await _productRepository.CreateAsync(
                command,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:products:public",
                cancellationToken);

            return Result<Guid>.Success(productId);
        }

        public async Task<Result> UpdateAsync(
            UpdateProductCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _productRepository.UpdateAsync(
                command,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:products:public",
                cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _productRepository.DeleteAsync(
                id,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:products:public",
                cancellationToken);

            return Result.Success();
        }
    }
}
