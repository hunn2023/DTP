using DTP.Modules.Auth.Application.Abstractions.Services;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Auth.Application.Queries.Users
{
    public class GetUsersQuery : IRequest<Result<PagedResultDto<UserDto>>>
    {
        public string? Keyword { get; set; }
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResultDto<UserDto>>>
    {
        private readonly IUserService _userService;

        public GetUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public Task<Result<PagedResultDto<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            return _userService.GetPagedAsync(
                request.Keyword,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
