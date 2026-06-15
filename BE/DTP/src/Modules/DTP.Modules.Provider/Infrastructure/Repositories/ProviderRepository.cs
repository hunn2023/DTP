using DTP.Modules.Provider.Application.Abstractions.Repositories;
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
    public class ProviderRepository : RepositoryBase<Domain.Entities.Provider>, IProviderRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderRepository(ProviderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Domain.Entities.Provider?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return _dbContext.Providers
                .FirstOrDefaultAsync(x => x.Code == normalizedCode, cancellationToken);
        }
    }
}
