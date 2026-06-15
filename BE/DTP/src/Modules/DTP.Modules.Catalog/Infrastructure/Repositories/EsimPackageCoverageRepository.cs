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
    public class EsimPackageCoverageRepository : RepositoryBase<EsimPackageCoverage>, IEsimPackageCoverageRepository
    {
        private readonly CatalogDbContext _dbContext;

        public EsimPackageCoverageRepository(CatalogDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyList<EsimPackageCoverage>> GetByEsimPackageIdAsync(
            Guid esimPackageId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.EsimPackageCoverages
                .AsNoTracking()
                .Where(x => x.EsimPackageId == esimPackageId)
                .OrderBy(x => x.CountryName)
                .ToListAsync(cancellationToken);
        }

        public Task<bool> ExistsAsync(
            Guid esimPackageId,
            Guid countryId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.EsimPackageCoverages
                .AnyAsync(
                    x => x.EsimPackageId == esimPackageId &&
                         x.CountryId == countryId,
                    cancellationToken);
        }
        public async Task DeleteByEsimPackageIdAsync(
            Guid esimPackageId,
            CancellationToken cancellationToken = default)
        {
            var items = await _dbContext.EsimPackageCoverages
                .Where(x => x.EsimPackageId == esimPackageId)
                .ToListAsync(cancellationToken);

            if (items.Count == 0)
                return;

            _dbContext.EsimPackageCoverages.RemoveRange(items);
        }
    }
}
