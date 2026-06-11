using DTP.Modules.Payment.Application.Queries.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Payment.Presentation.Controllers.Admin
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/payments")]
    public class AdminPaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminPaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetPaymentByIdQuery
                {
                    Id = id
                },
                cancellationToken);

            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

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
    }
}
