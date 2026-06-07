using DTP.Modules.Provider.Application.Commands.Provisioning;
using DTP.Modules.Provider.Application.Queries;
using DTP.Modules.Provider.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Provider.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/provider-orders")]
    public class AdminProviderOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminProviderOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] Guid? providerId,
            [FromQuery] ProviderOrderStatus? status,
            [FromQuery] string? keyword,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetProviderOrdersQuery
            {
                ProviderId = providerId,
                Status = status,
                Keyword = keyword,
                FromDate = fromDate,
                ToDate = toDate,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProviderOrderDetailQuery
                {
                    Id = id
                },
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("{id:guid}/retry")]
        public async Task<IActionResult> Retry(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new RetryProviderOrderCommand
                {
                    ProviderOrderId = id
                },
                cancellationToken);

            return Ok(result);
        }
    }
}
