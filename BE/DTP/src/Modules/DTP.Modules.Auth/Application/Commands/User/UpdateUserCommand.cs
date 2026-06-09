using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Auth.Application.Commands.User
{
    public class UpdateUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public UpdateUserDto Request { get; set; } = default!;
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return _userService.UpdateAsync(request.Id, request.Request, cancellationToken);
        }
    }
}
