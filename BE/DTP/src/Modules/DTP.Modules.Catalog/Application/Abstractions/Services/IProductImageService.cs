

namespace DTP.Modules.Catalog.Application.Abstractions.Services
{

    public interface IProductImageService
    {
        Task<Guid> CreateAsync(
            Guid productId,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Guid id,
            string imageUrl,
            string? altText,
            int sortOrder,
            bool isThumbnail,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
