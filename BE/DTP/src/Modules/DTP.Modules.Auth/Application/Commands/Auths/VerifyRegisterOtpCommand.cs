using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;

namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public class VerifyRegisterOtpCommand : IRequest<Result>
    {
        public VerifyOtpRequestDto Request { get; set; } = default!;
    }

    public class VerifyRegisterOtpCommandHandler : IRequestHandler<VerifyRegisterOtpCommand, Result>
    {
        private readonly IAuthService _authService;

        public VerifyRegisterOtpCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<Result> Handle(VerifyRegisterOtpCommand request, CancellationToken cancellationToken)
        {
            return _authService.VerifyRegisterOtpAsync(request.Request, cancellationToken);
        }
    }
}
