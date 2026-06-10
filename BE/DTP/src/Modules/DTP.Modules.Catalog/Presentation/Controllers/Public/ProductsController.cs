using DTP.Modules.Catalog.Application.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DTP.Modules.Catalog.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/catalog/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("home/esim-products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHomeEsimProducts(
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetHomeEsimProductsQuery(),
                cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetPublicProductsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("variants")]
        public async Task<IActionResult> GetListVariant(
           [FromQuery] GetProductVariantQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var result = await _mediator.Send(
                new GetPublicProductBySlugQuery(slug));

            return Ok(result);
        }
    }
}
