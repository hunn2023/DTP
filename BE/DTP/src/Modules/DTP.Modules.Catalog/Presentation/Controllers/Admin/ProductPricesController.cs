using DTP.Modules.Catalog.Application.Commands.ProductPrices;
using DTP.Modules.Catalog.Application.Queries.ProductPrices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/catalog/product-prices")]
    public class ProductPricesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductPricesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList([FromQuery] GetProductPricesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _mediator.Send(new GetProductPriceByIdQuery(id));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductPriceCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(new { id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductPriceCommand command)
        {
            command.Id = id;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductPriceCommand(id));
            return Ok(result);
        }
    }
}
