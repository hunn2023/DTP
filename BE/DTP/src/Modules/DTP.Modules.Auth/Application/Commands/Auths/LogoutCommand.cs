using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public class LogoutCommand : IRequest<Result>
    {
        public string RefreshToken { get; set; } = default!;
        public string? IpAddress { get; set; }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IAuthService _authService;

        public LogoutCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return _authService.LogoutAsync(
                request.RefreshToken,
                request.IpAddress,
                cancellationToken);
        }
    }
}
