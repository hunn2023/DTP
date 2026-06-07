using DTP.Modules.Content.Application.Commands.Seo;
using DTP.Modules.Content.Application.Queries.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/content/seo")]
    [Authorize]
    public class AdminSeoMetadataController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminSeoMetadataController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] string? entityType,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetSeoMetadataPagedQuery(
                    keyword,
                    entityType,
                    pageIndex,
                    pageSize),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetSeoMetadataByIdQuery(id),
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

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateSeoMetadataCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateSeoMetadataRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateSeoMetadataCommand(
                id,
                request.EntityType,
                request.EntityId,
                request.RoutePath,
                request.MetaTitle,
                request.MetaDescription,
                request.MetaKeywords,
                request.CanonicalUrl,
                request.OgTitle,
                request.OgDescription,
                request.OgImageUrl,
                request.Robots);

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new DeleteSeoMetadataCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    public class UpdateSeoMetadataRequest
    {
        public string EntityType { get; set; } = default!;

        public Guid? EntityId { get; set; }

        public string? RoutePath { get; set; }

        public string MetaTitle { get; set; } = default!;

        public string? MetaDescription { get; set; }

        public string? MetaKeywords { get; set; }

        public string? CanonicalUrl { get; set; }

        public string? OgTitle { get; set; }

        public string? OgDescription { get; set; }

        public string? OgImageUrl { get; set; }

        public string? Robots { get; set; }
    }
}
