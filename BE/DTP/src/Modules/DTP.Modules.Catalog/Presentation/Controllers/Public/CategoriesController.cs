using DTP.Modules.Catalog.Application.Queries.Categories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Catalog.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/catalog/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Public categories list
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetPublicCategoriesQuery query)
        {
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
