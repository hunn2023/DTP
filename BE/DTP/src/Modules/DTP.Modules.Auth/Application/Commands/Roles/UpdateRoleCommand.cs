using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Role
{
    public class UpdateRoleCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public UpdateRoleDto Request { get; set; } = default!;
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            return _roleService.UpdateAsync(request.Id, request.Request, cancellationToken);
        }
    }
}
