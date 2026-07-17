using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Infrastructure.Repositories
{
    public class ProductVariantRepository
     : RepositoryBase<ProductVariant>,
       IProductVariantRepository
    {
        private readonly CatalogDbContext _context;

        public ProductVariantRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

 
        public async Task<List<ProductVariant>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductVariants

                .Where(x => x.ProductId == productId && !x.IsDeleted)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ProductVariant>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductVariants
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsBySkuAsync(
            string sku,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.ProductVariants
                .AsNoTracking()
                .Where(x => x.Sku == sku.Trim());

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<ProductVariant?> GetBySkuAsync(string sku, CancellationToken cancellationToken = default)
        {
             return await _context.ProductVariants
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Sku == sku.Trim(), cancellationToken);
        }
    }
}
