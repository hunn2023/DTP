using DTP.Modules.Ordering.Application.Commands.CancelOrder;
using DTP.Modules.Ordering.Application.Commands.Checkout;
using DTP.Modules.Ordering.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(
            [FromBody] CheckoutRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();

            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers["User-Agent"].ToString();
            var referrerUrl = Request.Headers["Referer"].ToString();

            var command = new CheckoutCommand
            {
                UserId = userId,
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                ProductId = request.ProductId,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity,

                IpAddress = ipAddress,
                UserAgent = userAgent,
                ReferrerUrl = referrerUrl,
                CheckoutSource = "Web"
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }

        private string? GetClientIpAddress()
        {
            var cfIp = Request.Headers["CF-Connecting-IP"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(cfIp))
                return cfIp;

            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(forwardedFor))
                return forwardedFor.Split(',')[0].Trim();

            var realIp = Request.Headers["X-Real-IP"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(realIp))
                return realIp;

            return HttpContext.Connection.RemoteIpAddress?.ToString();
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery
            {
                OrderId = id,
                UserId = GetUserId(),
                IsAdmin = false
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
                UserId = GetUserId(),
                IsAdmin = false,
                Reason = request.Reason
            }, cancellationToken);

            return Ok(result);
        }



        private Guid GetUserId()
        {
            var value = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException("User is not authenticated.");

            return Guid.Parse(value);
        }
    }

    public class CheckoutRequest
    {
        public string CustomerEmail { get; set; } = default!;
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }

        public Guid ProductId { get; set; }
        public Guid? ProductVariantId { get; set; }

        public int Quantity { get; set; } = 1;

    }

    public class CancelOrderRequest
    {
        public string? Reason { get; set; }
    }
}
