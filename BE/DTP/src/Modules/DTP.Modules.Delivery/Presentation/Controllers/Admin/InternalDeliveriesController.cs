using DTP.Modules.Delivery.Application.Commands.DeliverOrder;
using DTP.Modules.Delivery.Application.Commands.Delivery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Delivery.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/internal/deliveries")]
    public class InternalDeliveriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InternalDeliveriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("create-from-paid-order")]
        public async Task<IActionResult> CreateFromPaidOrder(
            [FromBody] CreateDeliveryFromPaidOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var createResult = await _mediator.Send(
                new CreateDeliveryCommand(
                    request.OrderId,
                    ip),
                cancellationToken);

            if (!createResult.IsSuccess)
                return Ok(createResult);

            var processResult = await _mediator.Send(
                new ProcessDeliveryCommand(createResult.Data),
                cancellationToken);

            return Ok(processResult);
        }
    }

    public class CreateDeliveryFromPaidOrderRequest
    {
        public Guid OrderId { get; set; }
    }
}
