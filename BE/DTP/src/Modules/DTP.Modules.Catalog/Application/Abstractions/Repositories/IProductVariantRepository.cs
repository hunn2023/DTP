using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductVariantRepository : IRepositoryBase<ProductVariant>
    {
        Task<List<ProductVariant>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<List<ProductVariant>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsBySkuAsync(
            string sku,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<ProductVariant?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default);
    }
}
