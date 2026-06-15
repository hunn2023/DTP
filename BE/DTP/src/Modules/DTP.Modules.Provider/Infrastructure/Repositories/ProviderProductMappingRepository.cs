using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Repositories
{
    public class ProviderProductMappingRepository : RepositoryBase<ProviderProductMapping>, IProviderProductMappingRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderProductMappingRepository(ProviderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ProviderProductMapping?> GetByProviderSkuAsync(
            Guid providerId,
            string providerSku,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderProductMappings
                .FirstOrDefaultAsync(
                    x => x.ProviderId == providerId && x.ProviderSku == providerSku,
                    cancellationToken);
        }

        public Task<ProviderProductMapping?> GetByEsimPackageIdAsync(
            Guid esimPackageId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderProductMappings
                .FirstOrDefaultAsync(
                    x => x.EsimPackageId == esimPackageId,
                    cancellationToken);
        }

    }
}
