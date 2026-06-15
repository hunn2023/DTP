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
    public class ProviderOrderItemRepository : IProviderOrderItemRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderOrderItemRepository(ProviderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ProviderOrderItem?> GetByProviderOrderAndSkuAsync(
            Guid providerOrderId,
            string sku,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderOrderItems
                .FirstOrDefaultAsync(
                    x => x.ProviderOrderId == providerOrderId &&
                         x.Sku == sku,
                    cancellationToken);
        }

        public async Task AddAsync(
            ProviderOrderItem item,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.ProviderOrderItems.AddAsync(item, cancellationToken);
        }
    }
}
