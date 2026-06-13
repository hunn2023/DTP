using DTP.Modules.Content.Application.Queries.Pages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Content.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/content/pages")]
    public class PublicContentPagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicContentPagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(
            string slug,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetContentPageBySlugQuery(slug),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
