using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Repositories
{
    public class ProviderOrderRepository : IProviderOrderRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderOrderRepository(ProviderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ProviderOrder?> GetByDtpOrderIdAsync(
            Guid dtpOrderId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.DtpOrderId == dtpOrderId, cancellationToken);
        }

        public Task<ProviderOrder?> GetByProviderOrderPublicIdAsync(
            string providerOrderPublicId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(
                    x => x.ProviderOrderPublicId == providerOrderPublicId,
                    cancellationToken);
        }

        public async Task AddAsync(
            ProviderOrder providerOrder,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.ProviderOrders.AddAsync(providerOrder, cancellationToken);
        }
    }
}
