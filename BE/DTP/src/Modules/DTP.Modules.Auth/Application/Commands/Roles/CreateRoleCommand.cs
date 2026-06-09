using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Role
{
    public class CreateRoleCommand : IRequest<Result<Guid>>
    {
        public CreateRoleDto Request { get; set; } = default!;
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<Guid>>
    {
        private readonly IRoleService _roleService;

        public CreateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            return _roleService.CreateAsync(request.Request, cancellationToken);
        }
    }
}
