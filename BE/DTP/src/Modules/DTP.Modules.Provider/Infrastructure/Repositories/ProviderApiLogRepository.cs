using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Repositories
{
    public class ProviderApiLogRepository : RepositoryBase<ProviderApiLog>, IProviderApiLogRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderApiLogRepository(ProviderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
