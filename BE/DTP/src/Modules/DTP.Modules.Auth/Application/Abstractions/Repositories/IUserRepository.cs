using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Domain.Entities;
using DTP.Shared.Application.Pagination;
using DTP.Shared.Infrastructure.Persistence;


namespace DTP.Modules.Auth.Application.Abstractions.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User?> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default);

        Task<User?> GetByIdWithRolesAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByEmailAsync(
            string email,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<UserDto>> GetPagedAsync(
            string? keyword,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
