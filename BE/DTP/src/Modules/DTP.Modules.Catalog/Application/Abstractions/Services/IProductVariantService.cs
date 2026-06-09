using DTP.Modules.Catalog.Application.DTOs;
using DTP.Shared.Application;


namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductVariantService
    {
        Task<Result<List<ProductVariantDto>>> GetByProductIdAsync(
         Guid productId,
         CancellationToken cancellationToken = default);


        Task<Result<Guid>> CreateAsync(
           Guid productId,
           string? sku,
           string name,
           string? shortName,
           string? description,
           int sortOrder,
           bool isActive,
           CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string? sku,
            string name,
            string? shortName,
            string? description,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);
    }
}
