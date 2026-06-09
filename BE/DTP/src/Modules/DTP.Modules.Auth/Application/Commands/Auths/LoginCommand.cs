using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public class LoginCommand : IRequest<Result<LoginResponseDto>>
    {
        public LoginRequestDto Request { get; set; } = default!;
        public string? IpAddress { get; set; }
    }
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponseDto>>
    {
        private readonly IAuthService _authService;

        public LoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return _authService.LoginAsync(request.Request, request.IpAddress, cancellationToken);
        }
    }
}
