using DTP.Modules.Auth.Application.Abstractions.Services;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Permissions
{
    public class AssignPermissionsCommand : IRequest<bool>
    {
        public Guid RoleId { get; set; }
        public List<Guid> PermissionIds { get; set; } = new();
    }

    public class AssignPermissionsCommandHandler : IRequestHandler<AssignPermissionsCommand, bool>
    {
        private readonly IRoleService _roleService;

        public AssignPermissionsCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public Task<bool> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
        {
            return _roleService.AssignPermissionsAsync(
                request.RoleId,
                request.PermissionIds,
                cancellationToken);
        }
    }
}
