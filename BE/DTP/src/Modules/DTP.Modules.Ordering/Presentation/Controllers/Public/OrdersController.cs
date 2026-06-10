
using DTP.Modules.Ordering.Application.Commands.Orders;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace DTP.Modules.Ordering.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [EnableRateLimiting("ordering-create")]
        [HttpPost]
        public async Task<IActionResult> Create(
         [FromBody] CreateOrderCommand command,
         CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [EnableRateLimiting("ordering-create")]
        [HttpPost("{id:guid}/confirm")]
        public async Task<IActionResult> Confirm(
            Guid id,
            [FromBody] ConfirmOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new ConfirmOrderCommand
            {
                OrderId = id,
                PaymentMethod = request.PaymentMethod,
                ChangedBy = request.ChangedBy
            }, cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [EnableRateLimiting("ordering-read")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery
            {
                Id = id
            }, cancellationToken);

            return result.IsSuccess ? Ok(result) : NotFound(result);
        }


        [EnableRateLimiting("ordering-create")]
        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(
            Guid id,
            [FromBody] CancelOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CancelOrderCommand
            {
                OrderId = id,
                Reason = request.Reason,
                ChangedBy = request.ChangedBy
            }, cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
