using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Auth.Application.Abstractions.Repositories
{
    public interface IPermissionRepository : IRepositoryBase<Permission>
    {
        Task<List<Permission>> GetByRoleIdAsync(
            Guid roleId,
            CancellationToken cancellationToken = default);

        Task<List<string>> GetPermissionCodesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<List<Permission>> GetByModuleAsync(
            string module,
            CancellationToken cancellationToken = default);
    }
}
