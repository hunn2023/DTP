using DTP.Modules.Content.Application.Commands.Customers;
using DTP.Modules.Content.Application.Queries.Customers;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Customer.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/customers")]
    [Authorize]
    public class AdminCustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminCustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] CustomerStatus? status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomersPagedQuery
            {
                Keyword = keyword,
                Status = status,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetCustomerDetailQuery
            {
                Id = id
            }, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCustomerCommand command,
            CancellationToken cancellationToken = default)
        {
            command.Id = id;

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPatch("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(
            Guid id,
            [FromBody] ChangeCustomerStatusCommand command,
            CancellationToken cancellationToken = default)
        {
            command.Id = id;

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new DeleteCustomerCommand
            {
                Id = id
            }, cancellationToken);

            return Ok(result);
        }
    }
}
