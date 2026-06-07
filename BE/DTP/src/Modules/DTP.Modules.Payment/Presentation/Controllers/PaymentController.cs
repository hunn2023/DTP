using DTP.Modules.Payment.Application.Commands.VnptEpay;
using DTP.Modules.Payment.Application.Queries.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Presentation.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("orders/{orderId:guid}/status")]
        [Authorize]
        public async Task<IActionResult> GetByOrderId(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetPaymentByOrderIdQuery
            {
                OrderId = orderId
            }, cancellationToken);

            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost("vnpt-epay/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> VnptEpayCallback(
            [FromBody] VnptEpayCallbackRequest request,
            CancellationToken cancellationToken)
        {
            var rawBody = JsonSerializer.Serialize(request);

            var result = await _mediator.Send(new HandleVnptEpayCallbackCommand
            {
                OrderCode = request.OrderCode,
                TransactionCode = request.TransactionCode,
                ProviderTransactionCode = request.ProviderTransactionCode,
                Amount = request.Amount,
                Status = request.Status,
                Message = request.Message,
                Signature = request.Signature,
                RawBody = rawBody
            }, cancellationToken);

            return Ok(new
            {
                success = result
            });
        }
    }

    public class VnptEpayCallbackRequest
    {
        public string? OrderCode { get; set; }

        public string? TransactionCode { get; set; }

        public string? ProviderTransactionCode { get; set; }

        public decimal Amount { get; set; }

        public string? Status { get; set; }

        public string? Message { get; set; }

        public string? Signature { get; set; }
    }
}
