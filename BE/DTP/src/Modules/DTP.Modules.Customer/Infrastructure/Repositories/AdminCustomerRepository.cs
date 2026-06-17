using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Modules.Customer.Application.Abstractions.Repositories;
using DTP.Modules.Customer.Application.Common;
using DTP.Modules.Customer.Application.Constants;
using DTP.Modules.Customer.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Customer.Infrastructure.Repositories
{
    public class AdminCustomerRepository : IAdminCustomerRepository
    {
        private readonly AuthDbContext _authDbContext;

        public AdminCustomerRepository(AuthDbContext authDbContext)
        {
            _authDbContext = authDbContext;
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

                    // Tạm thời để 0. Sau này có thể nối sang Ordering module.
                    TotalOrders = 0,
                    TotalSpent = 0
                })
                .ToListAsync(cancellationToken);

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
            return await _authDbContext.Users
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

                    // Tạm thời để 0. Sau này có thể lấy từ Ordering module.
                    TotalOrders = 0,
                    TotalSpent = 0,
                    FirstOrderAt = null,
                    LastOrderAt = null
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
