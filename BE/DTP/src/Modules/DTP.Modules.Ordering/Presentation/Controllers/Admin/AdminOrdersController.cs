using DTP.Modules.Ordering.Application.Commands.CancelOrder;
using DTP.Modules.Ordering.Application.Queries;
using DTP.Modules.Ordering.Domain.Enums;
using DTP.Modules.Ordering.Presentation.Controllers.Public;
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

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery
            {
                OrderId = id,
                IsAdmin = true
            }, cancellationToken);

            return result == null ? NotFound() : Ok(result);
        }


        [HttpPost("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(
            Guid id,
            [FromBody] CancelOrderRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelOrderCommand
            {
                OrderId = id,
                UserId = Guid.Empty,
                IsAdmin = true,
                Reason = request.Reason
            }, cancellationToken);

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetPaged(
           [FromQuery] string? keyword,
           [FromQuery] OrderStatus? status,
           [FromQuery] OrderPaymentStatus? paymentStatus,
           [FromQuery] int pageIndex = 1,
           [FromQuery] int pageSize = 20,
           CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetAdminOrdersQuery
            {
                Keyword = keyword,
                Status = status,
                PaymentStatus = paymentStatus,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, cancellationToken);

            return Ok(result);
        }
    }
}
