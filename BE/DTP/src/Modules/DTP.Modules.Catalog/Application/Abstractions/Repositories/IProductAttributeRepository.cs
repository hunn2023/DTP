using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;

namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductAttributeRepository : IRepositoryBase<ProductAttribute>
    {
        Task<List<ProductAttribute>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}
