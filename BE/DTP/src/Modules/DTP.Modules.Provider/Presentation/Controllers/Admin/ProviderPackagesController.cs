using DTP.Modules.Provider.Application.Commands.Providers;
using DTP.Modules.Provider.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Presentation.Controllers.Admin
{
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    [Route("api/admin/provider-packages")]
    public class ProviderPackagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProviderPackagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync(
            [FromBody] SyncProviderPackagesRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new SyncProviderPackagesCommand
                {
                    ProviderCode = request.ProviderCode
                },
                cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] Guid? providerId,
            [FromQuery] string? keyword,
            [FromQuery] string? syncStatus,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetProviderPackagesQuery
                {
                    ProviderId = providerId,
                    Keyword = keyword,
                    SyncStatus = syncStatus,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                },
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProviderPackageDetailQuery
                {
                    Id = id
                },
                cancellationToken);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{id:guid}/activate")]
        public async Task<IActionResult> Activate(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new ActivateSyncedProviderPackageCommand
                {
                    ProviderPackageProductId = id
                },
                cancellationToken);

            return Ok(new
            {
                success = result
            });
        }
    }

    public class SyncProviderPackagesRequest
    {
        public string ProviderCode { get; set; } = default!;
    }
}
