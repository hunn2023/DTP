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
    public class ProductFaqRepository : RepositoryBase<ProductFaq>, IProductFaqRepository
    {
        private readonly CatalogDbContext _dbContext;

        public ProductFaqRepository(CatalogDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductFaq>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductFaqs
                .AsNoTracking()
                .Where(x => x.ProductId == productId);

            if (onlyActive)
            {
                query = query.Where(x => x.IsActive);
            }

            return await query
                .OrderBy(x => x.SortOrder)
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
   
    }
}
