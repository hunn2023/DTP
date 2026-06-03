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
    public class ProductImageRepository
    : RepositoryBase<ProductImage>,
      IProductImageRepository
    {
        private readonly CatalogDbContext _context;

        public ProductImageRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductImage>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductImages
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.IsThumbnail)
                .ThenBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);
        }
    }
}
