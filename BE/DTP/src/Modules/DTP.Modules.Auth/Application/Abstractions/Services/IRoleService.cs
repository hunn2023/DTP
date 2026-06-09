using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;


namespace DTP.Modules.Auth.Application.Abstractions.Services
{
    public interface IRoleService
    {
        Task<Result<List<RoleDto>>> GetListAsync(CancellationToken cancellationToken = default);

        Task<Result<RoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateAsync(CreateRoleDto request, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(Guid id, UpdateRoleDto request, CancellationToken cancellationToken = default);

        Task<Result> AssignPermissionsAsync(Guid roleId, List<Guid> permissionIds, CancellationToken cancellationToken = default);
    }
}
