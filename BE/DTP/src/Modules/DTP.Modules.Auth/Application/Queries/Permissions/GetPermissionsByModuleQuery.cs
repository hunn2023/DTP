using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using MediatR;

namespace DTP.Modules.Auth.Application.Queries.Permissions
{
    public class GetPermissionsByModuleQuery : IRequest<Dictionary<string, List<PermissionDto>>>
    {
    }

    public class GetPermissionsByModuleQueryHandler
    : IRequestHandler<GetPermissionsByModuleQuery, Dictionary<string, List<PermissionDto>>>
    {
        private readonly IPermissionService _permissionService;

        public GetPermissionsByModuleQueryHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public Task<Dictionary<string, List<PermissionDto>>> Handle(
            GetPermissionsByModuleQuery request,
            CancellationToken cancellationToken)
        {
            return _permissionService.GetByModuleAsync(cancellationToken);
        }
    }
}
