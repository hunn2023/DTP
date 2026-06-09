using DTP.Modules.Provider.Application.Commands.Mappings;
using DTP.Modules.Provider.Application.Queries;
using DTP.Modules.Provider.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace DTP.Modules.Provider.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/provider-mappings")]
    public class AdminProviderMappingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminProviderMappingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] Guid? providerId,
            [FromQuery] ProviderProductType? productType,
            [FromQuery] Guid? productId,
            [FromQuery] Guid? productVariantId,
            [FromQuery] bool? isActive,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetProviderMappingsQuery
            {
                ProviderId = providerId,
                ProductType = productType,
                ProductId = productId,
                ProductVariantId = productVariantId,
                IsActive = isActive,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateProviderProductMappingCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _mediator.Send(command, cancellationToken);

            return Ok(new { id });
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateProviderProductMappingCommand command,
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
                new DeleteProviderProductMappingCommand
                {
                    Id = id
                },
                cancellationToken);

            return Ok(result);
        }
    }
}
