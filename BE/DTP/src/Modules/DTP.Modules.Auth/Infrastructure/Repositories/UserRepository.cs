using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
namespace DTP.Modules.Auth.Infrastructure.Repositories
{
    public class UserRepository
       : RepositoryBase<User>,
         IUserRepository
    {
        private readonly AuthDbContext _context;

        public UserRepository(AuthDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            email = email.Trim().ToLower();

            return await _context.Users
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<User?> GetByIdWithRolesAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(
            string email,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            email = email.Trim().ToLower();

            var query = _context.Users.AsQueryable();

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(x => x.Email == email, cancellationToken);
        }

        public async Task<PagedResultDto<UserDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(x =>
                    x.Email.Contains(keyword) ||
                    x.FullName.Contains(keyword) ||
                    (x.Phone != null && x.Phone.Contains(keyword)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    Email = x.Email,
                    Phone = x.Phone,
                    FullName = x.FullName,
                    AvatarUrl = x.AvatarUrl,
                    IsActive = x.IsActive,
                    EmailConfirmed = x.EmailConfirmed
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<UserDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
