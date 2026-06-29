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


        public async Task<IReadOnlyList<Guid>> GetOrderIdsReadyForDeliveryAsync(
            int take,
            CancellationToken cancellationToken = default)
                {
                    var candidateOrderIds = await _dbContext.ProviderRedeems
                        .Where(x => x.Status == "Done" && !x.EmailSent)
                        .OrderBy(x => x.UpdatedAt ?? x.CreatedAt)
                        .Select(x => x.DtpOrderId)
                        .Distinct()
                        .Take(take)
                        .ToListAsync(cancellationToken);

                    var readyOrderIds = new List<Guid>();

                    foreach (var orderId in candidateOrderIds)
                    {
                        var redeems = await _dbContext.ProviderRedeems
                            .Where(x => x.DtpOrderId == orderId)
                            .ToListAsync(cancellationToken);

                        if (redeems.Count == 0)
                            continue;

                        var allDone = redeems.All(x => x.Status == "Done");

                        if (allDone)
                            readyOrderIds.Add(orderId);
                    }

                    return readyOrderIds;
                }

        public async Task MarkEmailSentByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var redeems = await _dbContext.ProviderRedeems
                .Where(x =>
                    x.DtpOrderId == orderId &&
                    x.Status == "Done" &&
                    !x.EmailSent)
                .ToListAsync(cancellationToken);

            foreach (var redeem in redeems)
            {
                redeem.MarkEmailSent();
            }
        }

        public async Task<IReadOnlyList<ProviderRedeem>> GetDoneNotEmailSentByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProviderRedeems
                .Where(x =>
                    x.DtpOrderId == orderId &&
                    x.Status == "Done" &&
                    !x.EmailSent)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
