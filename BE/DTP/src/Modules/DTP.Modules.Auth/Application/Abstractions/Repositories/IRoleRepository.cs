using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.Abstractions.Repositories
{
    public interface IRoleRepository : IRepositoryBase<Role>
    {
        Task<Role?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<Role?> GetByIdWithPermissionsAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<List<Role>> GetActiveRolesAsync(
            CancellationToken cancellationToken = default);
    }
}
