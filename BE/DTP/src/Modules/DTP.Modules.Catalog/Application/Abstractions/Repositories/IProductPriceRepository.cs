using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductPriceRepository : IRepositoryBase<ProductPrice>
    {
        Task<List<ProductPrice>> GetListAsync(
            Guid? productId,
            Guid? productVariantId,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsActivePriceAsync(
            Guid productId,
            Guid? productVariantId,
            string currency,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);
    }
}
