using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public class RegisterCommand : IRequest<Result>
    {
        public RegisterRequestDto Request { get; set; } = default!;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IAuthService _authService;

        public RegisterCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return _authService.RegisterAsync(
                request.Request,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
        }
    }
}
