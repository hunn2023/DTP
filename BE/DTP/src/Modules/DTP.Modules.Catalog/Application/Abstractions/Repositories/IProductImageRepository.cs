using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{

    public interface IProductImageRepository : IRepositoryBase<ProductImage>
    {

        Task<List<ProductImage>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<ProductImage?> GetThumbnailAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<int> GetNextSortOrderAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task ClearThumbnailAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

    }
}
