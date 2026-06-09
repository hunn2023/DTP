using DTP.Modules.Auth.Application.Queries.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/permissions")]
    [Authorize]
    public class PermissionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await _mediator.Send(new GetPermissionsQuery());
            return Ok(result);
        }

        [HttpGet("by-module")]
        public async Task<IActionResult> GetByModule()
        {
            var result = await _mediator.Send(new GetPermissionsByModuleQuery());
            return Ok(result);
        }
    }
}
