using DTP.Modules.Payment.Application.Queries.PaymentProviders;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/public/payment-providers")]
    public class PublicPaymentProvidersController : ControllerBase
    {
        private readonly ISender _sender;

        public PublicPaymentProvidersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> GetActivePaymentProviders(
            CancellationToken cancellationToken)
        {
            var result = await _sender.Send(
                new GetPublicPaymentProvidersQuery(),
                cancellationToken);

            return Ok(new
            {
                isSuccess = true,
                data = result
            });
        }
    }
}
