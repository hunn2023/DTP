using DTP.Modules.Payment.Application.Commands.Payment;
using DTP.Modules.Payment.Application.Commands.VnptEpay;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Application.Queries.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace DTP.Modules.Payment.Presentation.Controllers
{

    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[Authorize]
        [HttpGet("orders/{orderId:guid}")]
        public async Task<IActionResult> GetByOrderId(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetPaymentByOrderIdQuery
                {
                    OrderId = orderId
                },
                cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }


        //[EnableRateLimiting("payment-create-qr")]
        //[Authorize]
        [HttpPost("qr")]
        public async Task<IActionResult> CreateQr(
           [FromBody] CreatePaymentQrRequestDto request,
           CancellationToken cancellationToken)
        {
            var ip = GetIpAddress();

            var result = await _mediator.Send(
                new CreatePaymentQrCommand
                {
                    OrderId = request.OrderId,
                    IpAddress = ip
                },
                cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        private string GetIpAddress()
        {
            return HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor)
                ? forwardedFor.ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

    }
}
