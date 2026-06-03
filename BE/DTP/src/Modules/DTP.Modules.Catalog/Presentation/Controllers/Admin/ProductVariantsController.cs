using DTP.Modules.Catalog.Application.Commands.ProductVariant;
using DTP.Modules.Catalog.Application.Queries.ProductVariants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
   // [Authorize(Roles = "Admin")]
    [Route("api/admin/catalog/product-variants")]
    public class ProductVariantsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductVariantsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-product/{productId:guid}")]
        public async Task<IActionResult> GetList(
            Guid productId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetProductVariantsQuery(productId),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProductVariantCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProductVariantCommand command,
            CancellationToken cancellationToken)
        {
            command.Id = id;

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new DeleteProductVariantCommand(id),
                cancellationToken);

            return Ok(result);
        }
    }
}
