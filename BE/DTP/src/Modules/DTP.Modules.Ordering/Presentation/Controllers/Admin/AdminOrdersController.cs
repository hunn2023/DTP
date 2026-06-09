
using DTP.Modules.Ordering.Application.Commands.Orders;
using DTP.Modules.Ordering.Application.DTOs;
using DTP.Modules.Ordering.Application.Queries;
using DTP.Modules.Ordering.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Ordering.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/orders")]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminOrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
      [FromQuery] string? keyword,
      [FromQuery] Guid? customerId,
      [FromQuery] OrderStatus? status,
      [FromQuery] OrderPaymentStatus? paymentStatus,
      [FromQuery] int pageIndex = 1,
      [FromQuery] int pageSize = 20,
      CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetOrdersPagedQuery
            {
                Keyword = keyword,
                CustomerId = customerId,
                Status = status,
                PaymentStatus = paymentStatus,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, cancellationToken);

            return Ok(result);
        }

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

        [HttpPost("{id:guid}/mark-paid")]
        public async Task<IActionResult> MarkPaid(
            Guid id,
            [FromBody] MarkOrderPaidRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new MarkOrderPaidCommand
            {
                OrderId = id,
                PaymentTransactionId = request.PaymentTransactionId,
                ChangedBy = request.ChangedBy
            }, cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("{id:guid}/complete")]
        public async Task<IActionResult> Complete(
            Guid id,
            [FromBody] CompleteOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new CompleteOrderCommand
            {
                OrderId = id,
                ChangedBy = request.ChangedBy
            }, cancellationToken);

            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

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
