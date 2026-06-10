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

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeCountries(
            [FromQuery] string? region,
            [FromQuery] string? keyword,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 12,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetHomeCountriesQuery(
                    region,
                    keyword,
                    pageIndex,
                    pageSize),
                cancellationToken);

            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
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
