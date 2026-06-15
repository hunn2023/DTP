using DTP.Modules.Catalog.Application.Queries.ProductFaqs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/public/product-faqs")]
    public class PublicProductFaqController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicProductFaqController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-product/{productId:guid}")]
        public async Task<IActionResult> GetByProductId(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductFaqsByProductIdQuery(
                    productId,
                    onlyActive: true),
                cancellationToken);

            return Ok(result);
        }
    }
}
