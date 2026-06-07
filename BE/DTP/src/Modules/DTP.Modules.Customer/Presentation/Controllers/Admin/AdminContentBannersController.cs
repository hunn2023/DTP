using DTP.Modules.Content.Application.Commands.Banners;
using DTP.Modules.Content.Application.Queries.Banners;
using DTP.Modules.Content.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Content.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/content/banners")]
    [Authorize]
    public class AdminContentBannersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminContentBannersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] ContentBannerPosition? position,
            [FromQuery] bool? isActive,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetContentBannersPagedQuery(
                    keyword,
                    position,
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
                new GetContentBannerByIdQuery(id),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateContentBannerCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateContentBannerRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new UpdateContentBannerCommand(
                id,
                request.Title,
                request.ImageUrl,
                request.MobileImageUrl,
                request.LinkUrl,
                request.Description,
                request.Position,
                request.StartDate,
                request.EndDate,
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
                new EnableContentBannerCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPatch("{id:guid}/disable")]
        public async Task<IActionResult> Disable(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new DisableContentBannerCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new DisableContentBannerCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    public class UpdateContentBannerRequest
    {
        public string Title { get; set; } = default!;

        public string ImageUrl { get; set; } = default!;

        public string? MobileImageUrl { get; set; }

        public string? LinkUrl { get; set; }

        public string? Description { get; set; }

        public ContentBannerPosition Position { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }
    }