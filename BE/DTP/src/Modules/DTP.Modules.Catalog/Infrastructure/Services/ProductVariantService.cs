using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _variantRepository;
        private readonly IProductCacheInvalidator _cacheInvalidator;
        public ProductVariantService(IProductVariantRepository variantRepository, IProductCacheInvalidator cacheInvalidator)
        {
            _variantRepository = variantRepository;
            _cacheInvalidator = cacheInvalidator;
        }

        public async Task<Result<Guid>> CreateAsync(
            Guid productId,
            string? sku,
            string name,
            decimal price,
            decimal? originalPrice,
            int? durationDays,
            decimal? dataAmount,
            string? dataUnit,
            bool isUnlimited,
            int sortOrder,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result<Guid>.Failure("ProductId không hợp lệ.");

            if (price < 0)
                return Result<Guid>.Failure("Giá bán không hợp lệ.");

            if (originalPrice.HasValue && originalPrice.Value < 0)
                return Result<Guid>.Failure("Giá gốc không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(sku))
            {
                var existsSku = await _variantRepository.ExistsBySkuAsync(
                    sku,
                    null,
                    cancellationToken);

                if (existsSku)
                    return Result<Guid>.Failure("SKU đã tồn tại.");
            }

            var variant = new ProductVariant(
                productId,
                sku,
                name,
                price,
                originalPrice,
                durationDays,
                dataAmount,
                dataUnit,
                isUnlimited,
                sortOrder);

            await _variantRepository.AddAsync(variant, cancellationToken);
            await _variantRepository.SaveChangesAsync(cancellationToken);
            await _cacheInvalidator.ClearProductDetailAsync(productId, cancellationToken);

            return Result<Guid>.Success(variant.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            string? sku,
            string name,
            decimal price,
            decimal? originalPrice,
            int? durationDays,
            decimal? dataAmount,
            string? dataUnit,
            bool isUnlimited,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var variant = await _variantRepository.GetByIdAsync(id, cancellationToken);

            if (variant == null)
                return Result.Failure("Không tìm thấy biến thể sản phẩm.");

            if (price < 0)
                return Result.Failure("Giá bán không hợp lệ.");

            if (originalPrice.HasValue && originalPrice.Value < 0)
                return Result.Failure("Giá gốc không hợp lệ.");

            if (!string.IsNullOrWhiteSpace(sku))
            {
                var existsSku = await _variantRepository.ExistsBySkuAsync(
                    sku,
                    id,
                    cancellationToken);

                if (existsSku)
                    return Result.Failure("SKU đã tồn tại.");
            }

            variant.Update(
                sku,
                name,
                price,
                originalPrice,
                durationDays,
                dataAmount,
                dataUnit,
                isUnlimited,
                sortOrder,
                isActive);

            await _variantRepository.SaveChangesAsync(cancellationToken);
            await _cacheInvalidator.ClearProductDetailAsync(variant.ProductId, cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var variant = await _variantRepository.GetByIdAsync(id, cancellationToken);

            if (variant == null)
                return Result.Failure("Không tìm thấy biến thể sản phẩm.");

            _variantRepository.Remove(variant);
            await _variantRepository.SaveChangesAsync(cancellationToken);
            await _cacheInvalidator.ClearProductDetailAsync(variant.ProductId, cancellationToken);

            return Result.Success();
        }
    }
}
