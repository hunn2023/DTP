using DTP.Modules.Content.Application.Queries.Banners;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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