using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Modules.Ordering.Application.Abstractions.Services;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Shared.Application;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Ordering.Infrastructure.Services
{
    public class OrderingCatalogService : IOrderingCatalogService
    {
        private readonly CatalogDbContext _catalogDbContext;

        public OrderingCatalogService(CatalogDbContext catalogDbContext)
        {
            _catalogDbContext = catalogDbContext;
        }

        public async Task<Result<OrderingProductSnapshotDto?>> GetProductForCheckoutAsync(
            Guid productId,
            Guid? productVariantId,
            CancellationToken cancellationToken = default)
        {
            var product = await _catalogDbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == productId && !x.IsDeleted && x.IsActive,
                    cancellationToken);

            if (product == null) return Result<OrderingProductSnapshotDto?>.Failure("Product not found.");

            var variant = productVariantId.HasValue
                ? await _catalogDbContext.ProductVariants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == productVariantId.Value
                             && x.ProductId == productId
                             && !x.IsDeleted
                             && x.IsActive,
                        cancellationToken)
                : null;

            var price = await _catalogDbContext.ProductPrices
                .AsNoTracking()
                .Where(x =>
                    x.ProductId == productId
                    && x.ProductVariantId == productVariantId
                    && x.Currency == "VND"
                    && x.IsActive
                    && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (price == null) return Result<OrderingProductSnapshotDto?>.Failure("Product price not found.");

            var esimPackage = await _catalogDbContext.EsimPackages
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.ProductId == productId
                         && (productVariantId == null || x.ProductVariantId == productVariantId)
                         && !x.IsDeleted
                         && x.IsActive,
                    cancellationToken);

            //var phoneCard = await _catalogDbContext.PhoneCards
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(
            //        x => x.ProductId == productId
            //             && (productVariantId == null || x.ProductVariantId == productVariantId)
            //             && !x.IsDeleted
            //             && x.IsActive,
            //        cancellationToken);

            return new OrderingProductSnapshotDto
            {
                ProductId = product.Id,
                ProductVariantId = variant?.Id,

                EsimPackageId = esimPackage?.Id,
                //PhoneCardId = phoneCard?.Id,

                ProductCode = product.Code ?? string.Empty,
                ProductName = product.Name,
                ProductSlug = product.Slug,
                VariantName = variant?.Name,
                Sku = variant?.Sku,
                ThumbnailUrl = product.ThumbnailUrl,

                UnitPrice = price.SalePrice > 0 ? price.SalePrice : price.OriginalPrice,
                CurrencyCode = price.Currency,

                IsActive = product.IsActive && (variant == null || variant.IsActive)
            };
        }
    }
}
