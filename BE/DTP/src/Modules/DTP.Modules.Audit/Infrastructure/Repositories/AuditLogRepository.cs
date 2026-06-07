using DTP.Modules.Audit.Application.Abstractions.Repositories;
using DTP.Modules.Audit.Application.DTOs;
using DTP.Modules.Audit.Domain.Entities;
using DTP.Modules.Audit.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Infrastructure.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AuditDbContext _context;

        public AuditLogRepository(AuditDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken = default)
        {
            await _context.AuditLogs.AddAsync(auditLog, cancellationToken);
        }

        public async Task<AuditLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.AuditLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<PagedResultDto<AuditLogListItemDto>> GetPagedAsync(
            AuditLogFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var query = _context.AuditLogs
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
            {
                var keyword = filter.Keyword.Trim();

                query = query.Where(x =>
                    x.Module.Contains(keyword) ||
                    x.Action.Contains(keyword) ||
                    (x.UserName != null && x.UserName.Contains(keyword)) ||
                    (x.EntityName != null && x.EntityName.Contains(keyword)) ||
                    (x.Description != null && x.Description.Contains(keyword)) ||
                    (x.IpAddress != null && x.IpAddress.Contains(keyword)));
            }

            if (!string.IsNullOrWhiteSpace(filter.Module))
            {
                var module = filter.Module.Trim();
                query = query.Where(x => x.Module == module);
            }

            if (!string.IsNullOrWhiteSpace(filter.Action))
            {
                var action = filter.Action.Trim();
                query = query.Where(x => x.Action == action);
            }

            if (filter.ActionType.HasValue)
                query = query.Where(x => x.ActionType == filter.ActionType.Value);

            if (filter.Status.HasValue)
                query = query.Where(x => x.Status == filter.Status.Value);

            if (filter.UserId.HasValue)
                query = query.Where(x => x.UserId == filter.UserId.Value);

            if (!string.IsNullOrWhiteSpace(filter.EntityName))
            {
                var entityName = filter.EntityName.Trim();
                query = query.Where(x => x.EntityName == entityName);
            }

            if (filter.EntityId.HasValue)
                query = query.Where(x => x.EntityId == filter.EntityId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(x => x.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                query = query.Where(x => x.CreatedAt <= filter.ToDate.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((filter.PageIndex - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new AuditLogListItemDto
                {
                    Id = x.Id,
                    Module = x.Module,
                    Action = x.Action,
                    ActionType = x.ActionType,
                    Status = x.Status,
                    UserId = x.UserId,
                    UserName = x.UserName,
                    EntityName = x.EntityName,
                    EntityId = x.EntityId,
                    IpAddress = x.IpAddress,
                    RequestPath = x.RequestPath,
                    RequestMethod = x.RequestMethod,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<AuditLogListItemDto>(
                items,
                filter.PageIndex,
                filter.PageSize,
                totalCount);
        }
    }
}
