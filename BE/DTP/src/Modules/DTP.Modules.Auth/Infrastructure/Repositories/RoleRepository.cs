using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Auth.Infrastructure.Repositories
{
    public class RoleRepository
     : RepositoryBase<Role>,
       IRoleRepository
    {
        private readonly AuthDbContext _context;

        public RoleRepository(AuthDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Role?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            code = code.Trim().ToUpper();

            return await _context.Roles
                .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
        }

        public async Task<Role?> GetByIdWithPermissionsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .Include(x => x.RolePermissions)
                    .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            code = code.Trim().ToUpper();

            var query = _context.Roles.AsQueryable();

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(x => x.Code == code, cancellationToken);
        }

        public async Task<List<Role>> GetActiveRolesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.Roles
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
