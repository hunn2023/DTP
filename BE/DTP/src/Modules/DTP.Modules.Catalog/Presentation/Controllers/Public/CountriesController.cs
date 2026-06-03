using DTP.Modules.Catalog.Application.Queries.Countries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Catalog.Presentation.Controllers.Public
{
    [ApiController]
    [Route("api/catalog/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetPublicCountriesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
