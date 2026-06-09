using DTP.Modules.Audit.Application.Commands;
using DTP.Modules.Audit.Application.Queries;
using DTP.Modules.Audit.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DTP.Modules.Audit.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/audit/logs")]
    [Authorize]
    public class AdminAuditLogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminAuditLogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? keyword,
            [FromQuery] string? module,
            [FromQuery] string? action,
            [FromQuery] AuditActionType? actionType,
            [FromQuery] AuditStatus? status,
            [FromQuery] Guid? userId,
            [FromQuery] string? entityName,
            [FromQuery] Guid? entityId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetAuditLogsQuery
                {
                    Keyword = keyword,
                    Module = module,
                    Action = action,
                    ActionType = actionType,
                    Status = status,
                    UserId = userId,
                    EntityName = entityName,
                    EntityId = entityId,
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                },
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetAuditLogByIdQuery(id),
                cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAuditLogCommand command,
            CancellationToken cancellationToken = default)
        {
            var id = await _mediator.Send(command, cancellationToken);

            return Ok(new
            {
                id
            });
        }
    }
}
