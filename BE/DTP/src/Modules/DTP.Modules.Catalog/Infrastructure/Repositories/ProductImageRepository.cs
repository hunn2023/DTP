using DTP.Modules.Catalog.Application.Abstractions.Repositories;
using DTP.Modules.Catalog.Domain.Entities;
using DTP.Modules.Catalog.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


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

        public async Task<List<ProductImage>> GetByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductImages
                .Where(x => x.ProductId == productId && !x.IsDeleted)
                .OrderByDescending(x => x.IsThumbnail)
                .ThenBy(x => x.SortOrder)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductImage?> GetThumbnailAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProductImages
                .FirstOrDefaultAsync(
                    x => x.ProductId == productId
                         && x.IsThumbnail
                         && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<int> GetNextSortOrderAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var maxSortOrder = await _context.ProductImages
                .Where(x => x.ProductId == productId && !x.IsDeleted)
                .Select(x => (int?)x.SortOrder)
                .MaxAsync(cancellationToken);

            return (maxSortOrder ?? 0) + 1;
        }

        public async Task ClearThumbnailAsync(
            Guid productId,
            CancellationToken cancellationToken = default)
        {
            var images = await _context.ProductImages
                .Where(x => x.ProductId == productId
                            && x.IsThumbnail
                            && !x.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var image in images)
            {
                //image.SetThumbnail(false);
            }
        }

    }
}
