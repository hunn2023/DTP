using DTP.Modules.Payment.Application.Commands.Payment;
using DTP.Modules.Payment.Application.Commands.VnptEpay;
using DTP.Modules.Payment.Application.DTOs;
using DTP.Modules.Payment.Application.Queries.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize]
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

        [Authorize]
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

        [AllowAnonymous]
        [HttpPost("callback/vnpt-epay")]
        public async Task<IActionResult> VnptEpayCallback(
            [FromBody] VnptEpayCallbackDto callback,
            CancellationToken cancellationToken)
        {
            /*
             * Nếu VNPT yêu cầu đọc raw body chuẩn tuyệt đối để verify signature,
             * nên dùng EnableBuffering middleware hoặc custom action filter.
             */
            var rawBody = JsonSerializer.Serialize(callback);
            var ip = GetIpAddress();

            var result = await _mediator.Send(
                new HandleVnptEpayCallbackCommand
                {
                    Callback = callback,
                    RawBody = rawBody,
                    IpAddress = ip
                },
                cancellationToken);

            /*
             * Với callback provider, thường vẫn return 200 để tránh provider retry quá nhiều.
             * Body trả về nên đổi theo format VNPT yêu cầu.
             */
            if (!result.IsSuccess)
            {
                return Ok(new
                {
                    code = "01",
                    message = result.Error
                });
            }

            return Ok(new
            {
                code = "00",
                message = "Success"
            });
        }

        private string GetIpAddress()
        {
            return HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor)
                ? forwardedFor.ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

    }
}
