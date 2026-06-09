

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface ICatalogProductTypeCacheInvalidator
    {
        Task ClearEsimCacheAsync(
       CancellationToken cancellationToken = default);
        Task ClearPhoneCardCacheAsync(
       CancellationToken cancellationToken = default);


        Task ClearEsimDetailAsync(
       Guid esimPackageId,
       CancellationToken cancellationToken = default);


        Task ClearPhoneCardDetailAsync(
        Guid phoneCardId,
        CancellationToken cancellationToken = default);

        Task ClearAllCatalogSellingCacheAsync(
        CancellationToken cancellationToken = default);
    }
}
