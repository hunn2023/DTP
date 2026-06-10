
using DTP.Modules.Delivery.Application.Commands.Delivery;
using DTP.Modules.Delivery.Application.Queries;
using DTP.Modules.Delivery.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DTP.Shared.Application.Http;

namespace DTP.Modules.Delivery.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/deliveries")]
    [Authorize]
    public class AdminDeliveriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminDeliveriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] DeliveryStatus? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetDeliveriesPagedQuery(
                    keyword,
                    status,
                    page,
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
                new GetDeliveryByIdQuery(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("by-order/{orderId:guid}")]
        public async Task<IActionResult> GetByOrderId(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetDeliveryByOrderIdQuery(orderId),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{id:guid}/process")]
        public async Task<IActionResult> Process(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var ip = HttpContext.GetClientIp();

            var result = await _mediator.Send(
                new ProcessDeliveryCommand(id, ip),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{id:guid}/mark-delivered")]
        public async Task<IActionResult> MarkDelivered(
            Guid id,
            [FromBody] MarkDeliveredRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new MarkDeliveryDeliveredCommand(
                    id,
                    request.Note),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("{id:guid}/mark-failed")]
        public async Task<IActionResult> MarkFailed(
            Guid id,
            [FromBody] MarkFailedRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new MarkDeliveryFailedCommand(
                    id,
                    request.Error),
                cancellationToken);

            return Ok(result);
        }


        [HttpPost("{id:guid}/resend-esim-email")]
        public async Task<IActionResult> ResendEsimEmail(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new ResendEsimDeliveryEmailCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }

    public class MarkDeliveredRequest
    {
        public string? Note { get; set; }
    }

    public class MarkFailedRequest
    {
        public string Error { get; set; } = default!;
    }
}
