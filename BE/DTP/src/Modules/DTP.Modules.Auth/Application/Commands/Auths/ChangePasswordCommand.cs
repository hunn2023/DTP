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
    public class ChangePasswordCommand : IRequest<Result>
    {
        public Guid UserId { get; set; }

        public string CurrentPassword { get; set; } = default!;

        public string NewPassword { get; set; } = default!;

        public string ConfirmNewPassword { get; set; } = default!;

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public ChangePasswordRequestDto ToDto()
        {
            return new ChangePasswordRequestDto
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmNewPassword = ConfirmNewPassword
            };
        }
    }

    public class ChangePasswordCommandHandler
     : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IAuthService _authService;

        public ChangePasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(
            ChangePasswordCommand request,
            CancellationToken cancellationToken)
        {
            return await _authService.ChangePasswordAsync(
                request.UserId,
                request.ToDto(),
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
        }
    }
}
