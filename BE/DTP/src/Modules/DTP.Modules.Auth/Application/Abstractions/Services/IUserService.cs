using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;

namespace DTP.Modules.Auth.Application.Abstractions.Services
{
    public interface IUserService
    {
        Task<Result<PagedResultDto<UserDto>>> GetPagedAsync(string? keyword, int pageIndex, int pageSize, CancellationToken cancellationToken = default);

        Task<Result<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result<Guid>> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default);

        Task<Result> UpdateAsync(Guid id, UpdateUserDto request, CancellationToken cancellationToken = default);

        Task<Result> LockAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result> UnlockAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Result> AssignRolesAsync(Guid userId, List<Guid> roleIds, CancellationToken cancellationToken = default);
    }
}
