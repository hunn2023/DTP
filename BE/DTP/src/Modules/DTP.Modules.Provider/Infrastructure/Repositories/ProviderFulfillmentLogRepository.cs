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
    public class ProviderFulfillmentLogRepository : RepositoryBase<ProviderFulfillmentLog>, IProviderFulfillmentLogRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderFulfillmentLogRepository(ProviderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ProviderFulfillmentLog?> GetByOrderItemIdAsync(
            Guid orderItemId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderFulfillmentLogs
                .FirstOrDefaultAsync(x => x.OrderItemId == orderItemId, cancellationToken);
        }
    }
}
