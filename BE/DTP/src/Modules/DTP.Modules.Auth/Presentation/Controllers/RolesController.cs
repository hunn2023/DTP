using DTP.Modules.Auth.Application.Commands.Permissions;
using DTP.Modules.Auth.Application.Commands.Role;
using DTP.Modules.Auth.Application.DTOs;
using DTP.Modules.Auth.Application.Queries.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DTP.Modules.Auth.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/roles")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await _mediator.Send(new GetRolesQuery());
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery
            {
                Id = id
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleDto request)
        {
            var id = await _mediator.Send(new CreateRoleCommand
            {
                Request = request
            });

            return Ok(new
            {
                id,
                message = "Tạo role thành công."
            });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateRoleDto request)
        {
            var result = await _mediator.Send(new UpdateRoleCommand
            {
                Id = id,
                Request = request
            });

            return Ok(new
            {
                success = result,
                message = "Cập nhật role thành công."
            });
        }

        [HttpPost("{id:guid}/permissions")]
        public async Task<IActionResult> AssignPermissions(Guid id, AssignPermissionsDto request)
        {
            var result = await _mediator.Send(new AssignPermissionsCommand
            {
                RoleId = id,
                PermissionIds = request.PermissionIds
            });

            return Ok(new
            {
                success = result,
                message = "Gán permission thành công."
            });
        }
    }
}
