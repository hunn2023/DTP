using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Infrastructure.Repositories
{
    public class PermissionRepository
       : RepositoryBase<Permission>,
         IPermissionRepository
    {
        private readonly AuthDbContext _context;

        public PermissionRepository(AuthDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetByRoleIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default)
        {
            return await _context.RolePermissions
                .AsNoTracking()
                .Where(x => x.RoleId == roleId)
                .Select(x => x.Permission)
                .OrderBy(x => x.Module)
                .ThenBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetPermissionCodesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Code)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Permission>> GetByModuleAsync(
            string module,
            CancellationToken cancellationToken = default)
        {
            module = module.Trim().ToLower();

            return await _context.Permissions
                .AsNoTracking()
                .Where(x => x.Module.ToLower() == module)
                .OrderBy(x => x.Code)
                .ToListAsync(cancellationToken);
        }
    }
}
