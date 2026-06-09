using DTP.Shared.Application;


namespace DTP.Modules.Catalog.Application.Abstractions.Services
{
    public interface IProductPriceService
    {
        Task<Result<Guid>> CreateAsync(
             Guid productId,
             Guid? productVariantId,
             string currency,
             decimal originalPrice,
             decimal salePrice,
             decimal costPrice,
             DateTime? startDate,
             DateTime? endDate,
             string note,
             CancellationToken cancellationToken = default);

        Task<Result> DeleteProductPriceAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(
            Guid id,
            string currency,
            decimal originalPrice,
            decimal salePrice,
            decimal costPrice,
            DateTime? startDate,
            DateTime? endDate,
            bool isActive,
            string note,
            CancellationToken cancellationToken = default);
    }
}
