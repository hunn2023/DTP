

using DTP.Shared.Application;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductAttributeService
    {
        Task<Result<Guid>> CreateAsync(
            Guid productId,
            string name,
            string value,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string name,
            string value,
            int sortOrder,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
