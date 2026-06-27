using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Modules.Customer.Application.Abstractions.Repositories;
using DTP.Modules.Customer.Application.Common;
using DTP.Modules.Customer.Application.Constants;
using DTP.Modules.Customer.Application.DTOs;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Modules.Ordering.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Customer.Infrastructure.Repositories
{
    public class AdminCustomerRepository : IAdminCustomerRepository
    {
        private readonly AuthDbContext _authDbContext;

        private readonly OrderingDbContext _orderingDbContext;

        public AdminCustomerRepository(
            AuthDbContext authDbContext,
            OrderingDbContext orderingDbContext)
        {
            _authDbContext = authDbContext;
            _orderingDbContext = orderingDbContext;
        }


        public async Task<PagedResult<AdminCustomerListItemDto>> GetPagedAsync(
       string? keyword,
       bool? isActive,
       int pageIndex,
       int pageSize,
       CancellationToken cancellationToken = default)
        {
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var query = _authDbContext.Users
                .AsNoTracking()
                .Where(x => x.UserRoles.Any(ur => ur.Role.Code == CustomerRoleCodes.Customer));

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim().ToLower();

                query = query.Where(x =>
                    x.Email.ToLower().Contains(kw) ||
                    x.FullName.ToLower().Contains(kw) ||
                    (x.Phone != null && x.Phone.Contains(kw)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Email)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AdminCustomerListItemDto
                {
                    UserId = x.Id,
                    Email = x.Email,
                    Phone = x.Phone,
                    FullName = x.FullName,
                    AvatarUrl = x.AvatarUrl,
                    IsActive = x.IsActive,
                    EmailConfirmed = x.EmailConfirmed,
                    LastLoginAt = x.LastLoginAt,
                    Roles = x.UserRoles
                        .Select(ur => ur.Role.Code)
                        .ToList(),

                    TotalOrders = 0,
                    TotalSpent = 0
                })
                .ToListAsync(cancellationToken);

            if (items.Count == 0)
            {
                return new PagedResult<AdminCustomerListItemDto>
                {
                    Items = items,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalItems = totalItems
                };
            }

            var userIds = items
                .Select(x => x.UserId)
                .Distinct()
                .ToList();

            var orderStats = await _orderingDbContext.Orders
                .AsNoTracking()
                .Where(x => x.CustomerId.HasValue && userIds.Contains(x.CustomerId.Value))
                .GroupBy(x => x.CustomerId!.Value)
                .Select(g => new
                {
                    UserId = g.Key,

                    TotalOrders = g.Count(),

                    TotalSpent = g
                        .Where(x => x.PaymentStatus == OrderPaymentStatus.Paid)
                        .Sum(x => (decimal?)x.TotalAmount) ?? 0
                })
                .ToListAsync(cancellationToken);

            var orderStatsMap = orderStats.ToDictionary(x => x.UserId);

            foreach (var item in items)
            {
                if (orderStatsMap.TryGetValue(item.UserId, out var stat))
                {
                    item.TotalOrders = stat.TotalOrders;
                    item.TotalSpent = stat.TotalSpent;
                }
            }

            return new PagedResult<AdminCustomerListItemDto>
            {
                Items = items,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }

        public async Task<AdminCustomerDetailDto?> GetDetailAsync(
             Guid userId,
             CancellationToken cancellationToken = default)
        {
            var customer = await _authDbContext.Users
                .AsNoTracking()
                .Where(x =>
                    x.Id == userId &&
                    x.UserRoles.Any(ur => ur.Role.Code == CustomerRoleCodes.Customer))
                .Select(x => new AdminCustomerDetailDto
                {
                    UserId = x.Id,
                    Email = x.Email,
                    Phone = x.Phone,
                    FullName = x.FullName,
                    AvatarUrl = x.AvatarUrl,
                    IsActive = x.IsActive,
                    EmailConfirmed = x.EmailConfirmed,
                    LastLoginAt = x.LastLoginAt,
                    Roles = x.UserRoles
                        .Select(ur => ur.Role.Code)
                        .ToList(),

                    TotalOrders = 0,
                    TotalSpent = 0,
                    FirstOrderAt = null,
                    LastOrderAt = null
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (customer is null)
            {
                return null;
            }

            var orderStats = await _orderingDbContext.Orders
                .AsNoTracking()
                .Where(x => x.CustomerId == userId)
                .GroupBy(x => x.CustomerId)
                .Select(g => new
                {
                    TotalOrders = g.Count(),

                    TotalSpent = g
                        .Where(x => x.PaymentStatus == OrderPaymentStatus.Paid)
                        .Sum(x => (decimal?)x.TotalAmount) ?? 0,

                    FirstOrderAt = g
                        .Min(x => (DateTime?)x.CreatedAt),

                    LastOrderAt = g
                        .Max(x => (DateTime?)x.CreatedAt)
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (orderStats is not null)
            {
                customer.TotalOrders = orderStats.TotalOrders;
                customer.TotalSpent = orderStats.TotalSpent;
                customer.FirstOrderAt = orderStats.FirstOrderAt;
                customer.LastOrderAt = orderStats.LastOrderAt;
            }

            return customer;
        }
    }
}
