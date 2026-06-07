using DTP.Modules.Provider.Application.Abstractions.Repositories;
using DTP.Modules.Provider.Application.DTOs;
using DTP.Modules.Provider.Domain.Entities;
using DTP.Modules.Provider.Domain.Enums;
using DTP.Modules.Provider.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
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
        private readonly ProviderDbContext _context;

        public ProviderOrderRepository(ProviderDbContext context)
        {
            _context = context;
        }

        public async Task<ProviderOrder?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderOrders
                .Include(x => x.Items)
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<ProviderOrder?> GetByOrderIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            return await _context.ProviderOrders
                .Include(x => x.Items)
                .Include(x => x.Provider)
                .FirstOrDefaultAsync(x => x.OrderId == orderId && !x.IsDeleted, cancellationToken);
        }

        public async Task AddAsync(
            ProviderOrder order,
            CancellationToken cancellationToken = default)
        {
            await _context.ProviderOrders.AddAsync(order, cancellationToken);
        }

        public void Update(ProviderOrder order)
        {
            _context.ProviderOrders.Update(order);
        }


        public async Task<PagedResultDto<ProviderOrderDto>> GetPagedAsync(
    Guid? providerId,
    ProviderOrderStatus? status,
    string? keyword,
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

            var query = _context.ProviderOrders
                .AsNoTracking()
                .Include(x => x.Provider)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (providerId.HasValue && providerId.Value != Guid.Empty)
            {
                query = query.Where(x => x.ProviderId == providerId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.OrderCode.Contains(keyword) ||
                    (x.ProviderOrderCode != null && x.ProviderOrderCode.Contains(keyword)));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= toDate.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProviderOrderDto
                {
                    Id = x.Id,
                    OrderId = x.OrderId,
                    OrderCode = x.OrderCode,
                    ProviderId = x.ProviderId,
                    ProviderName = x.Provider.Name,
                    ProviderOrderCode = x.ProviderOrderCode,
                    Status = x.Status,
                    RetryCount = x.RetryCount,
                    ErrorCode = x.ErrorCode,
                    ErrorMessage = x.ErrorMessage,
                    SentAt = x.SentAt,
                    CompletedAt = x.CompletedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<ProviderOrderDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalItems
            };
        }
    }
}
