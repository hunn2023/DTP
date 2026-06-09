using DTP.Modules.Provider.Application.Commands.Providers;
using DTP.Modules.Provider.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/providers")]
    public class AdminProvidersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminProvidersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProvidersQuery(), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateExternalProviderCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateExternalProviderCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new DeleteExternalProviderCommand { Id = id },
                cancellationToken);

            return Ok(result);
        }
    }
}
