using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.DTOs;
using DTP.Modules.Audit.Domain.Enums;
using MediatR;

namespace DTP.Modules.Audit.Application.Commands
{
    public class CreateAuditLogCommand : IRequest<Guid>
    {
        public string Module { get; set; } = default!;

        public string Action { get; set; } = default!;

        public AuditActionType ActionType { get; set; } = AuditActionType.Unknown;

        public AuditStatus Status { get; set; } = AuditStatus.Success;

        public Guid? UserId { get; set; }

        public string? UserName { get; set; }

        public string? EntityName { get; set; }

        public Guid? EntityId { get; set; }

        public string? Description { get; set; }

        public string? OldValues { get; set; }

        public string? NewValues { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string? RequestPath { get; set; }

        public string? RequestMethod { get; set; }

        public string? CorrelationId { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class CreateAuditLogCommandHandler
        : IRequestHandler<CreateAuditLogCommand, Guid>
    {
        private readonly IAuditLogService _auditLogService;

        public CreateAuditLogCommandHandler(
            IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<Guid> Handle(
            CreateAuditLogCommand request,
            CancellationToken cancellationToken)
        {
            var dto = new CreateAuditLogDto
            {
                Module = request.Module,
                Action = request.Action,
                ActionType = request.ActionType,
                Status = request.Status,
                UserId = request.UserId,
                UserName = request.UserName,
                EntityName = request.EntityName,
                EntityId = request.EntityId,
                Description = request.Description,
                OldValues = request.OldValues,
                NewValues = request.NewValues,
                IpAddress = request.IpAddress,
                UserAgent = request.UserAgent,
                RequestPath = request.RequestPath,
                RequestMethod = request.RequestMethod,
                CorrelationId = request.CorrelationId,
                ErrorMessage = request.ErrorMessage
            };

            return await _auditLogService.CreateAsync(dto, cancellationToken);
        }
    }
}
