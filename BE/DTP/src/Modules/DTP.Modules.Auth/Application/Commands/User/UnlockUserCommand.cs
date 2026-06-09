using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Auth.Application.Commands.User
{
    public class UnlockUserCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }

    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Result>
    {
        private readonly IUserService _userService;

        public UnlockUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<Result> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            return _userService.UnlockAsync(request.Id, cancellationToken);
        }
    }
}
