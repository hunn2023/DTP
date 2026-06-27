using DTP.Modules.Catalog.Application.Commands.EsimPackages;
using DTP.Modules.Catalog.Application.Queries.EsimPackages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/admin/catalog/esim-packages")]
    public class EsimPackagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EsimPackagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetAdminEsimPackagesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetEsimPackageByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEsimPackageCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEsimPackageCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteEsimPackageCommand(id));
            return Ok(result);
        }
    }
}
