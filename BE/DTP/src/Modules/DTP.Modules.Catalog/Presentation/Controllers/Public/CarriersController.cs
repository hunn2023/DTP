using DTP.Modules.Catalog.Application.Commands.Carriers;
using DTP.Modules.Catalog.Application.Queries.Carriers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Catalog.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/catalog/carriers")]
    public class CarriersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CarriersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetPublicCarriersQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
