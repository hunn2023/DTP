using DTP.Modules.Content.Application.Queries.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/content/seo")]
    public class PublicSeoMetadataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicSeoMetadataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-route")]
        public async Task<IActionResult> GetByRoute(
            [FromQuery] string routePath,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetSeoMetadataByRoutePathQuery(routePath),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("by-entity")]
        public async Task<IActionResult> GetByEntity(
            [FromQuery] string entityType,
            [FromQuery] Guid entityId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetSeoMetadataByEntityQuery(entityType, entityId),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
    }
}
