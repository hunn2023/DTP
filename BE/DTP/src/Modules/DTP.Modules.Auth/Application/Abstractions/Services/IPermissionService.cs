using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;

namespace DTP.Modules.Auth.Application.Abstractions.Services
{
    public interface IPermissionService
    {
        Task<Result<List<PermissionDto>>> GetListAsync(CancellationToken cancellationToken = default);

        Task<Result<Dictionary<string, List<PermissionDto>>>> GetByModuleAsync(CancellationToken cancellationToken = default);
    }
}
