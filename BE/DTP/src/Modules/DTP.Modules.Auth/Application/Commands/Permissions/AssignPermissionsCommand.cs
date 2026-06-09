using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Permissions
{
    public class AssignPermissionsCommand : IRequest<Result>
    {
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand, Result>
    {
        private readonly IRoleService _roleService;

        public AssignPermissionsCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public Task<Result> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
        {
            return _roleService.AssignPermissionsAsync(
                request.RoleId,
                request.PermissionIds,
                cancellationToken);
        }
    }
}
