using Azure.Core;
using DTP.Modules.Payment.Application.Abstractions.Services;
using DTP.Modules.Payment.Application.Commands.Sepay;
using DTP.Modules.Payment.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/webhooks/sepay")]
    public sealed class SepayWebhookController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ISepayWebhookVerifier _sepayWebhookVerifier;

        public SepayWebhookController(
            ISender sender,
            ISepayWebhookVerifier sepayWebhookVerifier)
        {
            _sender = sender;
            _sepayWebhookVerifier = sepayWebhookVerifier;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Receive(CancellationToken cancellationToken)
        {
           Request.EnableBuffering();

            using var reader = new StreamReader(
                Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            var rawBody = await reader.ReadToEndAsync(cancellationToken);

            Request.Body.Position = 0;

            var signature = Request.Headers["X-SePay-Signature"].FirstOrDefault();

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            // 1. Verify chữ ký SePay.
            // Nếu bạn đang test local và chưa bật HMAC thì có thể cấu hình RequireWebhookSignature = false.
            var isValidSignature = _sepayWebhookVerifier.Verify(
                Request.Headers,
                rawBody);

            if (!isValidSignature)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Invalid signature"
                });
            }

            SepayWebhookDto? callback;

            try
            {
                callback = JsonSerializer.Deserialize<SepayWebhookDto>(
                    rawBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid JSON"
                });
            }

            if (callback == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid payload"
                });
            }

            var result = await _sender.Send(
                new HandleSepayWebhookCommand(
                    Callback: callback,
                    RawBody: rawBody,
                    Signature: signature,
                    IpAddress: ipAddress),
                cancellationToken);

            if (!result.IsSuccess)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        success = false,
                        message = result.Error
                    });
            }

            // Quan trọng:
            // SePay webhook thành công phải trả JSON có success = true.
            return Ok(new
            {
                success = true
            });
        }
    }
}
