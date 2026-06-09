using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public class RefreshTokenCommand : IRequest<Result<LoginResponseDto>>
    {
        public string RefreshToken { get; set; } = default!;
        public string? IpAddress { get; set; }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginResponseDto>>
    {
        private readonly IAuthService _authService;

        public RefreshTokenCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<Result<LoginResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return _authService.RefreshTokenAsync(
                request.RefreshToken,
                request.IpAddress,
                cancellationToken);
        }
    }
}
