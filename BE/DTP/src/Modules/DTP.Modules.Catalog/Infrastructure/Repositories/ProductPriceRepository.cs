using Azure.Core;
using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class ProductPriceRepository
     : RepositoryBase<ProductPrice>,
       IProductPriceRepository
    {
        private readonly CatalogDbContext _context;

        public ProductPriceRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductPrice>> GetListAsync(
            Guid? productId,
            Guid? productVariantId,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ProductPrices
                 .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .AsQueryable();

            if (productId.HasValue)
            {
                query = query.Where(x => x.ProductId == productId.Value);
            }

            if (productVariantId.HasValue)
            {
                query = query.Where(x => x.ProductVariantId == productVariantId.Value);
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsActivePriceAsync(
             Guid productId,
             Guid? productVariantId,
             string? currency,
             Guid? excludeId,
             CancellationToken cancellationToken = default)
        {
            var query = _context.ProductPrices
                .AsNoTracking()
                .Where(x =>
                    x.ProductId == productId &&
                    x.ProductVariantId == productVariantId &&
                    x.IsActive);

            if (!string.IsNullOrWhiteSpace(currency))
            {
                var normalizedCurrency = currency.Trim().ToUpper();

                query = query.Where(x => x.Currency == normalizedCurrency);
            }

            if (excludeId.HasValue && excludeId.Value != Guid.Empty)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }


        public async Task<List<ProductPrice>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductPrices
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.IsActive)
                .ThenBy(x => x.ProductVariantId)
                .ThenBy(x => x.SalePrice)
                .ToListAsync(cancellationToken);
        }



        public async Task<ProductPrice?> GetActiveByProductVariantAsync(
            Guid productId,
            Guid? productVariantId,
            string? currency,
            CancellationToken cancellationToken = default)
        {
            return  await _context.ProductPrices
                .AsNoTracking()
                .Where(x =>
                    x.ProductId == productId &&
                    x.ProductVariantId == productVariantId &&
                    x.IsActive &&
                    (string.IsNullOrEmpty(currency) || x.Currency == currency)
                    && !x.IsDeleted
                    )
                .OrderByDescending(x => x.Priority)
                .ThenByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

    }
}
