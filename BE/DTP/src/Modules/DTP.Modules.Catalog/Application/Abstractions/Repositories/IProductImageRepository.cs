using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{

    public interface IProductImageRepository : IRepositoryBase<ProductImage>
    {
        Task<List<ProductImage>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}
