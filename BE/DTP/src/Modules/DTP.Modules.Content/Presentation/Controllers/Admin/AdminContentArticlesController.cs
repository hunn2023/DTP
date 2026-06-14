using DTP.Modules.Content.Application.Commands.Articles;
using DTP.Modules.Content.Application.Queries.Articles;
using DTP.Modules.Content.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace DTP.Modules.Content.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/content/articles")]
    //[Authorize]
    public class AdminContentArticlesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminContentArticlesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] string? categoryCode,
            [FromQuery] ContentArticleStatus? status,
            [FromQuery] bool? isFeatured,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetContentArticlesPagedQuery(
                    keyword,
                    categoryCode,
                    status,
                    isFeatured,
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
                new GetContentArticleByIdQuery(id),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateContentArticleCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateContentArticleRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateContentArticleCommand(
                id,
                request.Title,
                request.Slug,
                request.Summary,
                request.Content,
                request.ThumbnailUrl,
                request.AuthorName,
                request.CategoryCode,
                request.Tags,
                request.Status,
                request.IsFeatured,
                request.SortOrder);

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/publish")]
        public async Task<IActionResult> Publish(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new PublishContentArticleCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/hide")]
        public async Task<IActionResult> Hide(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new HideContentArticleCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/featured")]
        public async Task<IActionResult> MarkAsFeatured(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new MarkContentArticleFeaturedCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/unfeatured")]
        public async Task<IActionResult> UnmarkAsFeatured(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new UnmarkContentArticleFeaturedCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new HideContentArticleCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    public class UpdateContentArticleRequest
    {
        public string Title { get; set; } = default!;

        public string Slug { get; set; } = default!;

        public string? Summary { get; set; }

        public string Content { get; set; } = default!;

        public string? ThumbnailUrl { get; set; }

        public string? AuthorName { get; set; }

        public string? CategoryCode { get; set; }

        public string? Tags { get; set; }

        public ContentArticleStatus Status { get; set; }

        public bool IsFeatured { get; set; }

        public int SortOrder { get; set; }
    }
}