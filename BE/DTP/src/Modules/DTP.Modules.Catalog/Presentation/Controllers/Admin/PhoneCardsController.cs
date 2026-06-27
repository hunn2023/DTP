using DTP.Modules.Catalog.Application.Commands.PhoneCards;
using DTP.Modules.Catalog.Application.Queries.PhoneCards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
    [Authorize(Roles = "ADMIN")]
    [Route("api/admin/catalog/phone-cards")]
    public class PhoneCardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PhoneCardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetAdminPhoneCardsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetPhoneCardByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePhoneCardCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePhoneCardCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePhoneCardCommand(id));
            return Ok(result);
        }
    }
}
