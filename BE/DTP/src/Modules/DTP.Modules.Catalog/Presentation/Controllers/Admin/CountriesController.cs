using DTP.Modules.Catalog.Application.Commands.Countries;
using DTP.Modules.Catalog.Application.Queries.Countries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DTP.Modules.Catalog.Presentation.Controllers.Admin
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/admin/catalog/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetAdminCountriesQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateCountryCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);
            return Ok(id);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            [FromQuery] GetCountryByIdQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }


        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateCountryCommand command,
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
                new DeleteCountryCommand(id),
                cancellationToken);

            return Ok(result);
        }

        [HttpPost("upload")]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Upload(
          Guid countryId,
          [FromForm] UploadCountryFlagCommand request,
          CancellationToken cancellationToken)
        {
            var command = new UploadCountryFlagCommand
            {
                CountryId = countryId,
                File = request.File,
            };

            var result = await _mediator.Send(command, cancellationToken);

            return Ok(result);
        }
    }
}
