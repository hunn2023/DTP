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
    public class ProductAttributeRepository
       : RepositoryBase<ProductAttribute>,
         IProductAttributeRepository
    {
        private readonly CatalogDbContext _context;

        public ProductAttributeRepository(CatalogDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<ProductAttribute>> GetListAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductAttributes
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
