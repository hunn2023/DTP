using DTP.Modules.Auth.Application.Abstractions.Repositories;
using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Shared.Application;
using Microsoft.EntityFrameworkCore;


namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly AuthDbContext _context;

        public RoleService(
            IRoleRepository roleRepository,
            AuthDbContext context)
        {
            _roleRepository = roleRepository;
            _context = context;
        }

        public async Task<Result<List<RoleDto>>> GetListAsync(
            CancellationToken cancellationToken = default)
        {
            var roles = await _context.Roles
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new RoleDto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive
                })
                .ToListAsync(cancellationToken);

            return Result<List<RoleDto>>.Success(roles);
        }

        public async Task<Result<RoleDto>> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(
                id,
                cancellationToken);

            if (role == null)
                return Result<RoleDto>.Failure("Không tìm thấy role.");

            return Result<RoleDto>.Success(MapToRoleDto(role));
        }

        private RoleDto MapToRoleDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Code = role.Code,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive,
                Permissions = role.RolePermissions
                    .Select(x => new PermissionDto
                    {
                        Id = x.Permission.Id,
                        Code = x.Permission.Code,
                        Name = x.Permission.Name,
                        Module = x.Permission.Module,
                        Description = x.Permission.Description
                    })
                    .ToList()
            };
        }

        public async Task<Result<Guid>> CreateAsync(
            CreateRoleDto request,
            CancellationToken cancellationToken = default)
        {
            var code = request.Code.Trim().ToUpper();

            var exists = await _roleRepository.ExistsByCodeAsync(
                code,
                null,
                cancellationToken);

            if (exists)
                return Result<Guid>.Failure("Mã role đã tồn tại."); ;


            var role = new Role
            {
                Code = code,
                Name = request.Name.Trim(),
                Description = request.Description,
                IsActive = true,
                IsSystem = false
            };

            await _roleRepository.AddAsync(role, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(role.Id);
        }

        public async Task<Result> UpdateAsync(
            Guid id,
            UpdateRoleDto request,
            CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdAsync(id, cancellationToken);

            if (role == null)
                return Result.Failure("Không tìm thấy role.");

            if (role.IsSystem)
                return Result.Failure("Không được sửa role hệ thống.");

            role.Name = request.Name.Trim();
            role.Description = request.Description;
            role.IsActive = request.IsActive;

            _roleRepository.Update(role);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> AssignPermissionsAsync(
            Guid roleId,
            List<Guid> permissionIds,
            CancellationToken cancellationToken = default)
        {
            var role = await _roleRepository.GetByIdWithPermissionsAsync(
                roleId,
                cancellationToken);

            if (role == null)
                return Result.Failure("Không tìm thấy role.");

            var distinctPermissionIds = permissionIds.Distinct().ToList();

            var validPermissionCount = await _context.Permissions
                .CountAsync(x => distinctPermissionIds.Contains(x.Id), cancellationToken);

            if (validPermissionCount != distinctPermissionIds.Count)
                return Result.Failure("Danh sách permission không hợp lệ.");

            var oldPermissions = await _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .ToListAsync(cancellationToken);

            _context.RolePermissions.RemoveRange(oldPermissions);

            foreach (var permissionId in distinctPermissionIds)
            {
                await _context.RolePermissions.AddAsync(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                }, cancellationToken);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
