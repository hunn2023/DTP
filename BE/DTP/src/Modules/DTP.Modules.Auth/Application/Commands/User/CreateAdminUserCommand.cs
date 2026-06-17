using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Application.Commands.User
{
    public class CreateAdminUserCommand : IRequest<Result<Guid>>
    {
        public CreateAdminUserRequestDto Request { get; set; } = new();

        public Guid CreatedByUserId { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }
    }

    public class CreateAdminUserCommandHandler
        : IRequestHandler<CreateAdminUserCommand, Result<Guid>>
    {
        private readonly IAuthService _authService;

        public CreateAdminUserCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<Result<Guid>> Handle(
            CreateAdminUserCommand command,
            CancellationToken cancellationToken)
        {
            return await _authService.CreateAdminUserAsync(
                command.Request,
                command.CreatedByUserId,
                command.IpAddress,
                command.UserAgent,
                cancellationToken);
        }
    }
}
