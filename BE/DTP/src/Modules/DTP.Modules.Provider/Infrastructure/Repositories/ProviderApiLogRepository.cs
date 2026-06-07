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
    public class ProviderApiLogRepository : IProviderApiLogRepository
    {
        private readonly ProviderDbContext _context;

        public ProviderApiLogRepository(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderApiLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderApiLogs
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(
                    x => x.Id == id && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<PagedResultDto<ProviderApiLogDto>> GetPagedAsync(
            Guid? providerId,
            ProviderApiLogType? logType,
            bool? isSuccess,
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

            var query = _context.ProviderApiLogs
                .AsNoTracking()
                .Include(x => x.Provider)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (providerId.HasValue && providerId.Value != Guid.Empty)
            {
                query = query.Where(x => x.ProviderId == providerId.Value);
            }

            if (logType.HasValue)
            {
                query = query.Where(x => x.LogType == logType.Value);
            }

            if (isSuccess.HasValue)
            {
                query = query.Where(x => x.IsSuccess == isSuccess.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.RequestedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.RequestedAt <= toDate.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.RequestedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProviderApiLogDto
                {
                    Id = x.Id,
                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,
                    LogType = x.LogType,
                    Endpoint = x.Endpoint,
                    Method = x.Method,
                    RequestBody = x.RequestBody,
                    ResponseBody = x.ResponseBody,
                    StatusCode = x.StatusCode,
                    IsSuccess = x.IsSuccess,
                    ErrorMessage = x.ErrorMessage,
                    RequestedAt = x.RequestedAt,
                    RespondedAt = x.RespondedAt,
                    DurationMs = x.DurationMs
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProviderApiLogDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task AddAsync(
            ProviderApiLog log,
            CancellationToken cancellationToken = default)
        {
            await _context.ProviderApiLogs.AddAsync(log, cancellationToken);
        }

        public void Update(ProviderApiLog log)
        {
            _context.ProviderApiLogs.Update(log);
        }

        public void Remove(ProviderApiLog log)
        {
            log.Delete();
            _context.ProviderApiLogs.Update(log);
        }
    }
}
