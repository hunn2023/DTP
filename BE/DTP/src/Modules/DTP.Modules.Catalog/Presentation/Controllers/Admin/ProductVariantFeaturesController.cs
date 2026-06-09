using DTP.Modules.Catalog.Application.Commands.ProductVariantFeatures;
using DTP.Modules.Catalog.Application.Queries.ProductVariantFeatures;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/catalog/product-variant-features")]
    public class ProductVariantFeaturesController : ControllerBase
    {

        private readonly IMediator _mediator;

        public ProductVariantFeaturesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-variant/{productVariantId:guid}")]
        public async Task<IActionResult> GetByProductVariantId(
            Guid productVariantId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductVariantFeaturesQuery(productVariantId),
                cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductVariantFeatureCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductVariantFeatureCommand command,
            CancellationToken cancellationToken)
        {
            if (id != command.Id)
                return BadRequest("Id không khớp.");

            var result = await _mediator.Send(command, cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new DeleteProductVariantFeatureCommand(id),
                cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
