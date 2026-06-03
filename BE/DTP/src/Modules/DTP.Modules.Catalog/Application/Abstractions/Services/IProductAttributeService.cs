

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductAttributeService
    {
        Task<Guid> CreateAsync(
            Guid productId,
            string name,
            string value,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Guid id,
            string name,
            string value,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
