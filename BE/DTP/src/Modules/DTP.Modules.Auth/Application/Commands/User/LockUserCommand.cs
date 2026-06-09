using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Auth.Application.Commands.User
{
    public class LockUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, Result>
    {
        private readonly IUserService _userService;

        public LockUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<Result> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            return _userService.LockAsync(request.Id, cancellationToken);
        }
    }
}
