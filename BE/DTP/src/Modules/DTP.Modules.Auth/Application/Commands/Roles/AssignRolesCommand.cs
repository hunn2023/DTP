using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Auth.Application.Commands.Role
{
    public class AssignRolesCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }
        public List<Guid> RoleIds { get; set; } = new();
    }

    public class AssignRolesCommandHandler : IRequestHandler<AssignRolesCommand, Result>
    {
        private readonly IUserService _userService;

        public AssignRolesCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<Result> Handle(AssignRolesCommand request, CancellationToken cancellationToken)
        {
            return _userService.AssignRolesAsync(
                request.UserId,
                request.RoleIds,
                cancellationToken);
        }
    }
}
