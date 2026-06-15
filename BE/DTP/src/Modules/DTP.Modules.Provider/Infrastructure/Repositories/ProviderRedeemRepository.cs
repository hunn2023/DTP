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
    public class ProviderRedeemRepository : IProviderRedeemRepository
    {
        private readonly ProviderDbContext _dbContext;

        public ProviderRedeemRepository(ProviderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<ProviderRedeem?> GetBySerialAsync(
            string serial,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.ProviderRedeems
                .FirstOrDefaultAsync(x => x.Serial == serial, cancellationToken);
        }

        public async Task<IReadOnlyList<ProviderRedeem>> GetPendingAsync(
            int take,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProviderRedeems
                .Where(x => x.Status == "Init" || x.Status == "Processing")
                .OrderBy(x => x.LastCheckedAt ?? x.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ProviderRedeem>> GetDoneNotEmailSentAsync(
            int take,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProviderRedeems
                .Where(x => x.Status == "Done" && !x.EmailSent)
                .OrderBy(x => x.UpdatedAt ?? x.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            ProviderRedeem redeem,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.ProviderRedeems.AddAsync(redeem, cancellationToken);
        }
    }
}
