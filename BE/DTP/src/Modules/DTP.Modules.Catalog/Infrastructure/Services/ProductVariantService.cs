using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Application.Abstractions.Services;
using DTP.Modules.Catalog.Application.DTOs;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Shared.Application;
namespace DTP.Modules.Catalog.Infrastructure.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _variantRepository;
        private readonly IProductCacheInvalidator _cacheInvalidator;

        public ProductVariantService(
            IProductVariantRepository variantRepository,
            IProductCacheInvalidator cacheInvalidator)
        {
            _variantRepository = variantRepository;
            _cacheInvalidator = cacheInvalidator;
        }


        public async Task<Result<List<ProductVariantDto>>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return new Result<List<ProductVariantDto>>();

            var variants = await _variantRepository.GetByProductIdAsync(
                productId,
                cancellationToken);

            return Result<List<ProductVariantDto>>.Success(variants
                .OrderBy(x => x.SortOrder)
                .Select(x => new ProductVariantDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Sku = x.Sku,
                    Name = x.Name,
                    ShortName = x.ShortName,
                    Description = x.Description,
                    SortOrder = x.SortOrder,
                    IsActive = x.IsActive
                })
                .ToList());


        }


        public async Task<Result<Guid>> CreateAsync(
            Guid productId,
            string? sku,
            string name,
            string? shortName,
            string? description,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (productId == Guid.Empty)
                return Result<Guid>.Failure("ProductId không hợp lệ.");

            if (string.IsNullOrWhiteSpace(name))
                return Result<Guid>.Failure("Tên biến thể không được để trống.");

            sku = sku?.Trim();
            name = name.Trim();
            shortName = shortName?.Trim();
            description = description?.Trim();

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
                shortName,
                description,
                sortOrder,
                isActive);

            await _variantRepository.AddAsync(variant, cancellationToken);
            await _variantRepository.SaveChangesAsync(cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(
                productId,
                cancellationToken);

            return Result<Guid>.Success(variant.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            string? sku,
            string name,
            string? shortName,
            string? description,
            int sortOrder,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id biến thể không hợp lệ.");

            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure("Tên biến thể không được để trống.");

            var variant = await _variantRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (variant == null)
                return Result.Failure("Không tìm thấy biến thể sản phẩm.");

            sku = sku?.Trim();
            name = name.Trim();
            shortName = shortName?.Trim();
            description = description?.Trim();

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
                shortName,
                description,
                sortOrder,
                isActive);

            await _variantRepository.SaveChangesAsync(cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(
                variant.ProductId,
                cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
                return Result.Failure("Id biến thể không hợp lệ.");

            var variant = await _variantRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (variant == null)
                return Result.Failure("Không tìm thấy biến thể sản phẩm.");

            _variantRepository.Remove(variant);

            await _variantRepository.SaveChangesAsync(cancellationToken);

            await _cacheInvalidator.ClearProductDetailAsync(
                variant.ProductId,
                cancellationToken);

            return Result.Success();
        }



    }
}
