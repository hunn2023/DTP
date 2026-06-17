using DTP.Modules.Auth.Application.Commands.Role;
using DTP.Modules.Auth.Application.Commands.User;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DTP.Modules.Auth.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] GetUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery
            {
                Id = id
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserDto request)
        {
            var id = await _mediator.Send(new CreateUserCommand
            {
                Request = request
            });

            return Ok(new
            {
                id,
                message = "Tạo user thành công."
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateUserDto request)
        {
            var result = await _mediator.Send(new UpdateUserCommand
            {
                Id = id,
                Request = request
            });

            return Ok(new
            {
                success = result,
                message = "Cập nhật user thành công."
            });
        }

        [HttpPost("{id:guid}/lock")]
        public async Task<IActionResult> Lock(Guid id)
        {
            var result = await _mediator.Send(new LockUserCommand
            {
                Id = id
            });

            return Ok(new
            {
                success = result,
                message = "Đã khóa user."
            });
        }

        [HttpPost("{id:guid}/unlock")]
        public async Task<IActionResult> Unlock(Guid id)
        {
            var result = await _mediator.Send(new UnlockUserCommand
            {
                Id = id
            });

            return Ok(new
            {
                success = result,
                message = "Đã mở khóa user."
            });
        }

        [HttpPost("{id:guid}/assign-roles")]
        public async Task<IActionResult> AssignRoles(Guid id, AssignRolesDto request)
        {
            var result = await _mediator.Send(new AssignRolesCommand
            {
                UserId = id,
                RoleIds = request.RoleIds
            });

            return Ok(new
            {
                success = result,
                message = "Gán role thành công."
            });
        }



        [HttpPost("admins")]
        public async Task<IActionResult> CreateAdminUser(
            [FromBody] CreateAdminUserRequestDto request,
            CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == Guid.Empty)
                return Unauthorized(new
                {
                    success = false,
                    message = "Không xác định được user hiện tại."
                });

            var command = new CreateAdminUserCommand
            {
                Request = request,
                CreatedByUserId = currentUserId,
                IpAddress = GetClientIp(),
                UserAgent = Request.Headers.UserAgent.ToString()
            };

            var result = await _mediator.Send(
                command,
                cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        private Guid GetCurrentUserId()
        {
            var userId =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("sub")
                ?? User.FindFirstValue("userId")
                ?? User.FindFirstValue("UserId");

            return Guid.TryParse(userId, out var id)
                ? id
                : Guid.Empty;
        }

        private string? GetClientIp()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();

            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }
    }
}
