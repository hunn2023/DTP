using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Auth.Application.Commands.Auths
{
    public class ForgotPasswordCommand : IRequest<Result>
    {
        public string Email { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }


    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IAuthService _authService;

        public ForgotPasswordCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result> Handle(
            ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var dto = new ForgotPasswordRequestDto
            {
                Email = request.Email
            };

            return await _authService.ForgotPasswordAsync(
                dto,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
        }
    }
}
