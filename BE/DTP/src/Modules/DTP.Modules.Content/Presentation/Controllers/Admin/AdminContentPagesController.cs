using DTP.Modules.Content.Application.Commands.Pages;
using DTP.Modules.Content.Application.Queries.Pages;
using DTP.Modules.Content.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Content.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/content/pages")]
    [Authorize(Roles = "ADMIN")]
    public class AdminContentPagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminContentPagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] ContentPageStatus? status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetContentPagesPagedQuery(keyword, status, pageIndex, pageSize),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetContentPageByIdQuery(id),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateContentPageCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateContentPageRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateContentPageCommand(
                id,
                request.Title,
                request.Slug,
                request.Summary,
                request.Content,
                request.ThumbnailUrl,
                request.Status,
                request.SortOrder);

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Hide(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new HideContentPageCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    public class UpdateContentPageRequest
    {
        public string Title { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Summary { get; set; }
        public string Content { get; set; } = default!;
        public string? ThumbnailUrl { get; set; }
        public ContentPageStatus Status { get; set; }
        public int SortOrder { get; set; }
    }
}
