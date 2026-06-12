using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public class ResetPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }


    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IAuthService _authService;

        public ResetPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var dto = new ResetPasswordRequestDto
            {
                Email = request.Email,
                OtpCode = request.OtpCode,
                NewPassword = request.NewPassword
            };

            return await _authService.ResetPasswordAsync(
                dto,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
        }
    }
}
