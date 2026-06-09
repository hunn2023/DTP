using DTP.Modules.Catalog.Application.Commands.PhoneCards;
using DTP.Modules.Catalog.Application.Queries.PhoneCards;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Catalog.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/catalog/phone-cards")]
    public class PhoneCardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PhoneCardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetPublicPhoneCardsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _mediator.Send(
                new GetPublicPhoneCardBySlugQuery(slug));

            return Ok(result);
        }
    }
}
