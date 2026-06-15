using DTP.Modules.Content.Application.Commands.Articles;
using DTP.Modules.Content.Application.Queries.Articles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Content.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/content/articles")]
    public class PublicContentArticlesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicContentArticlesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] string? categoryCode,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetPublicContentArticlesPagedQuery(
                    keyword,
                    categoryCode,
                    pageIndex,
                    pageSize),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured(
            [FromQuery] int take = 6,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetFeaturedContentArticlesQuery(take),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(
            string slug,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetContentArticleBySlugQuery(
                    slug,
                    true),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

       
    }
}
