

using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductAttributeService
    {
        Task<Result<List<ProductAttributeDto>>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateAsync(
            Guid productId,
            string key,
            string? displayName,
            string value,
            int sortOrder,
            bool isVisible,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string key,
            string? displayName,
            string value,
            int sortOrder,
            bool isVisible,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
