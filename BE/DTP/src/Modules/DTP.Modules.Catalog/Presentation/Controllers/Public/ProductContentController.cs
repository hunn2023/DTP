using DTP.Modules.Catalog.Application.Queries.ProductContents;
using DTP.Modules.Catalog.Domain.Enums;
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
    [Route("api/public/product-contents")]
    public class PublicProductContentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicProductContentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-product/{productId:guid}")]
        public async Task<IActionResult> GetByProductId(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductContentsByProductIdQuery(
                    productId,
                    onlyActive: true),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("by-product/{productId:guid}/type/{contentType}")]
        public async Task<IActionResult> GetByProductIdAndType(
            Guid productId,
            ProductContentType contentType,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductContentsByProductIdAndTypeQuery(
                    productId,
                    contentType,
                    onlyActive: true),
                cancellationToken);

            return Ok(result);
        }
    }
}
