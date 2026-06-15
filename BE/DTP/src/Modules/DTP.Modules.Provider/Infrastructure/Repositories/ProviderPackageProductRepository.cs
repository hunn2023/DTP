using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Provider.Infrastructure.Repositories
{
    public class ProviderPackageProductRepository : RepositoryBase<ProviderPackageProduct>, IProviderPackageProductRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderPackageProductRepository(ProviderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ProviderPackageProduct?> GetByProviderSkuAsync(
            Guid providerId,
            string providerSku,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderPackageProducts
                .FirstOrDefaultAsync(
                    x => x.ProviderId == providerId && x.ProviderSku == providerSku,
                    cancellationToken);
        }

        public async Task<(IReadOnlyList<ProviderPackageProduct> Items, int Total)> GetPagedAsync(
            Guid? providerId,
            string? keyword,
            string? syncStatus,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProviderPackageProducts
                .AsNoTracking()
                .AsQueryable();

            if (providerId.HasValue)
            {
                query = query.Where(x => x.ProviderId == providerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var key = keyword.Trim();

                query = query.Where(x =>
                    x.ProviderSku.Contains(key) ||
                    x.Name.Contains(key) ||
                    (x.Model != null && x.Model.Contains(key)));
            }

            if (!string.IsNullOrWhiteSpace(syncStatus))
            {
                var status = syncStatus.Trim();

                query = query.Where(x => x.SyncStatus == status);
            }

            var total = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.LastSyncedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, total);
        }
    }
}
