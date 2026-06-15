using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Domain.Enums;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Catalog.Application.Abstractions.Repositories
{
    public interface IProductContentRepository : IRepositoryBase<ProductContent>
    {

        Task<List<ProductContent>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);

        Task<List<ProductContent>> GetByProductIdAndTypeAsync(
            Guid productId,
            ProductContentType contentType,
            bool onlyActive = false,
            CancellationToken cancellationToken = default);
    }
}
