using DTP.Modules.Content.Application.Queries.Banners;
using DTP.Modules.Content.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Content.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/content/banners")]
    public class PublicContentBannersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicContentBannersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailable(
            [FromQuery] ContentBannerPosition? position,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetAvailableContentBannersQuery(position),
                cancellationToken);

            return Ok(result);
        }
    }
}