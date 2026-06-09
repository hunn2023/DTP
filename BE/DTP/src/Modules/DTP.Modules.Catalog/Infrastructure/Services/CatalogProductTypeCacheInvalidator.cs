using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.CacheKeys;
using DTP.Shared.Caching;

namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class CatalogProductTypeCacheInvalidator : ICatalogProductTypeCacheInvalidator
    {
        private readonly ICacheService _cacheService;

        public CatalogProductTypeCacheInvalidator(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task ClearEsimCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                EsimPackageCacheKeys.Prefix,
                cancellationToken);
        }

        public async Task ClearPhoneCardCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                PhoneCardCacheKeys.Prefix,
                cancellationToken);
        }

        public async Task ClearEsimDetailAsync(
            Guid esimPackageId,
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveAsync(
                EsimPackageCacheKeys.Detail(esimPackageId),
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                EsimPackageCacheKeys.ListPrefix,
                cancellationToken);
        }

        public async Task ClearPhoneCardDetailAsync(
            Guid phoneCardId,
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveAsync(
                PhoneCardCacheKeys.Detail(phoneCardId),
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                PhoneCardCacheKeys.ListPrefix,
                cancellationToken);
        }

        public async Task ClearAllCatalogSellingCacheAsync(
            CancellationToken cancellationToken = default)
        {
            await _cacheService.RemoveByPrefixAsync(
                ProductCacheKeys.Prefix,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                EsimPackageCacheKeys.Prefix,
                cancellationToken);

            await _cacheService.RemoveByPrefixAsync(
                PhoneCardCacheKeys.Prefix,
                cancellationToken);
        }
    }
}
