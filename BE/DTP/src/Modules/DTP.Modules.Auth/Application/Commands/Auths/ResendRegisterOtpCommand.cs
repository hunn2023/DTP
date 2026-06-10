using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public sealed record ResendRegisterOtpCommand(
        string Email,
        string? IpAddress,
        string? UserAgent) : IRequest<Result>;


    public class ResendRegisterOtpCommandHandler
      : IRequestHandler<ResendRegisterOtpCommand, Result>
    {
        private readonly IAuthService _authService;

        public ResendRegisterOtpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(
            ResendRegisterOtpCommand request,
            CancellationToken cancellationToken)
        {
            return await _authService.ResendRegisterOtpAsync(
                new ResendRegisterOtpRequestDto
                {
                    Email = request.Email
                },
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
        }
    }
}
