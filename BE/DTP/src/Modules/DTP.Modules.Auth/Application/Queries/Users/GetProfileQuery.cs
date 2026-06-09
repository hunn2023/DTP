using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using MediatR;


namespace DTP.Modules.Auth.Application.Queries.Users
{
    public class GetProfileQuery : IRequest<Result<UserDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<UserDto>>
    {
        private readonly IAuthService _authService;

        public GetProfileQueryHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public Task<Result<UserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            return _authService.GetProfileAsync(request.UserId, cancellationToken);
        }
    }
}
