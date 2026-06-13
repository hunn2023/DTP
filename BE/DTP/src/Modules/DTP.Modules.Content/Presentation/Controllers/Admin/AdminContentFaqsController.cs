using DTP.Modules.Content.Application.Commands.Faqs;
using DTP.Modules.Content.Application.Queries.Faqs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Content.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/content/faqs")]
    [Authorize]
    public class AdminContentFaqsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminContentFaqsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] string? categoryCode,
            [FromQuery] bool? isActive,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetContentFaqsPagedQuery(
                    keyword,
                    categoryCode,
                    isActive,
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
                new GetContentFaqByIdQuery(id),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateContentFaqCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateContentFaqRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateContentFaqCommand(
                id,
                request.Question,
                request.Answer,
                request.CategoryCode,
                request.SortOrder,
                request.IsActive);

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/enable")]
        public async Task<IActionResult> Enable(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new EnableContentFaqCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/disable")]
        public async Task<IActionResult> Disable(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new DisableContentFaqCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new DisableContentFaqCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    public class UpdateContentFaqRequest
    {
        public string Question { get; set; } = default!;

        public string Answer { get; set; } = default!;

        public string? CategoryCode { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }
}
