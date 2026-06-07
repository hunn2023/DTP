using DTP.Modules.Content.Application.Queries.Faqs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Content.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/content/faqs")]
    public class PublicContentFaqsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicContentFaqsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetActive(
            [FromQuery] string? categoryCode,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetActiveContentFaqsQuery(categoryCode),
                cancellationToken);

            return Ok(result);
        }
    }
}
