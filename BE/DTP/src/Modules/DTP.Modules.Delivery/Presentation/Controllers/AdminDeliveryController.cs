using DTP.Modules.Delivery.Application.Commands.DeliverOrder;
using DTP.Modules.Delivery.Application.Commands.ImportEsimProfiles;
using DTP.Modules.Delivery.Application.DTOs;
using DTP.Modules.Delivery.Application.Queries;
using DTP.Modules.Delivery.Domain.Enums;

namespace DTP.Modules.Delivery.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/delivery")]
    [Authorize(Roles = "Admin")]
    public class AdminDeliveryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminDeliveryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("esim-profiles")]
        public async Task<IActionResult> GetEsimProfiles(
            [FromQuery] Guid? productId,
            [FromQuery] Guid? productVariantId,
            [FromQuery] EsimProfileStatus? status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetEsimProfilesQuery
            {
                ProductId = productId,
                ProductVariantId = productVariantId,
                Status = status,
                PageIndex = pageIndex,
                PageSize = pageSize
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost("esim-profiles/import")]
        public async Task<IActionResult> ImportEsimProfiles(
            [FromBody] ImportEsimProfilesRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ImportEsimProfilesCommand
            {
                Items = request.Items
            }, cancellationToken);

            return Ok(new
            {
                imported = result
            });
        }

        [HttpPost("orders/{orderId:guid}/deliver")]
        public async Task<IActionResult> DeliverOrder(
            Guid orderId,
            [FromBody] DeliverOrderRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeliverOrderCommand
            {
                OrderId = orderId,
                Note = request.Note
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("orders/{orderId:guid}")]
        public async Task<IActionResult> GetDeliveryByOrderId(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetDeliveryByOrderIdQuery
            {
                OrderId = orderId
            }, cancellationToken);

            return Ok(result);
        }
    }

    public class ImportEsimProfilesRequest
    {
        public List<ImportEsimProfileDto> Items { get; set; } = new();
    }

    public class DeliverOrderRequest
    {
        public string? Note { get; set; }
    }
}
