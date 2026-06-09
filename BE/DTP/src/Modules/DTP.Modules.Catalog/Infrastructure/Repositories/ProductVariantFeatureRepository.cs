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
    public class ProductVariantFeatureRepository : RepositoryBase<ProductVariantFeature>, IProductVariantFeatureRepository
    {
        private readonly CatalogDbContext _context;

        public ProductVariantFeatureRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }



        public async Task<List<ProductVariantFeature>> GetByProductVariantIdAsync(
            Guid productVariantId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductVariantFeatures
                .AsNoTracking()
                .Where(x => x.ProductVariantId == productVariantId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Text)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetNextSortOrderAsync(
            Guid productVariantId,
            CancellationToken cancellationToken = default)
        {
            var maxSortOrder = await _context.ProductVariantFeatures
                .Where(x => x.ProductVariantId == productVariantId)
                .MaxAsync(x => (int?)x.SortOrder, cancellationToken);

            return (maxSortOrder ?? 0) + 1;
        }

 
    }
}
