using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using MediatR;


namespace DTP.Modules.Auth.Application.Queries.Permissions
{
    public class GetPermissionsQuery : IRequest<List<PermissionDto>>
    {
    }

    public class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, List<PermissionDto>>
    {
        private readonly IPermissionService _permissionService;

        public GetPermissionsQueryHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public Task<List<PermissionDto>> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            return _permissionService.GetListAsync(cancellationToken);
        }
    }
}
