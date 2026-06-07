using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Auth.Application.Commands.User
{
    public class CreateUserCommand : IRequest<Result<Guid>>
    {
        public CreateUserDto Request { get; set; } = default!;
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return _userService.CreateAsync(request.Request, cancellationToken);
        }
    }
}
