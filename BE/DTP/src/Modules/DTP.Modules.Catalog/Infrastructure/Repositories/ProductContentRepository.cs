using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Domain.Enums;
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
    public class ProductContentRepository : RepositoryBase<ProductContent>,
          IProductContentRepository
    {
        private readonly CatalogDbContext _dbContext;

        public ProductContentRepository(CatalogDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductContent>> GetByProductIdAsync(
            Guid productId,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductContents
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

        public async Task<List<ProductContent>> GetByProductIdAndTypeAsync(
            Guid productId,
            ProductContentType contentType,
            bool onlyActive = false,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductContents
                .AsNoTracking()
                .Where(x =>
                    x.ProductId == productId &&
                    x.ContentType == contentType);

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
