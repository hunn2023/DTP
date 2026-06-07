using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Domain.Enums;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Infrastructure.Repositories
{
    public class ProviderWebhookLogRepository : IProviderWebhookLogRepository
    {
        private readonly ProviderDbContext _context;

        public ProviderWebhookLogRepository(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderWebhookLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderWebhookLogs
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<PagedResultDto<ProviderWebhookLogDto>> GetPagedAsync(
            Guid? providerId,
            ProviderWebhookStatus? status,
            DateTime? fromDate,
            DateTime? toDate,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = 20;

            var query =
                from log in _context.ProviderWebhookLogs.AsNoTracking()
                join provider in _context.ExternalProviders.AsNoTracking()
                    on log.ProviderId equals provider.Id
                where !log.IsDeleted
                select new
                {
                    Log = log,
                    ProviderName = provider.Name
                };

            if (providerId.HasValue && providerId.Value != Guid.Empty)
            {
                query = query.Where(x => x.Log.ProviderId == providerId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Log.Status == status.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.Log.ReceivedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.Log.ReceivedAt <= toDate.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.Log.ReceivedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProviderWebhookLogDto
                {
                    Id = x.Log.Id,
                    ProviderId = x.Log.ProviderId,
                    ProviderName = x.ProviderName,
                    EventType = x.Log.EventType,
                    Payload = x.Log.Payload,
                    Status = x.Log.Status,
                    ErrorMessage = x.Log.ErrorMessage,
                    ReceivedAt = x.Log.ReceivedAt,
                    ProcessedAt = x.Log.ProcessedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProviderWebhookLogDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task AddAsync(
            ProviderWebhookLog log,
            CancellationToken cancellationToken = default)
        {
            await _context.ProviderWebhookLogs.AddAsync(log, cancellationToken);
        }

        public void Update(ProviderWebhookLog log)
        {
            _context.ProviderWebhookLogs.Update(log);
        }
    }
}
