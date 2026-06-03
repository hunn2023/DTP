using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductCacheInvalidator : IProductCacheInvalidator
    {
        private readonly ICacheService _cacheService;

        public ProductCacheInvalidator(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task ClearAllProductCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                ProductCacheKeys.Prefix,
                cancellationToken);
        }

        public async Task ClearProductDetailAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveAsync(
                ProductCacheKeys.Detail(productId),
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                ProductCacheKeys.ListPrefix,
                cancellationToken);
        }

        public async Task ClearRelatedCatalogCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                ProductCacheKeys.Prefix,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:esim:",
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                "catalog:phonecards:",
                cancellationToken);
        }
    }
}
