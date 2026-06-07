using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.DTOs;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Shared.Application.Pagination;
using MediatR;

namespace DTP.Modules.Audit.Application.Queries
{
    public class GetAuditLogsQuery : IRequest<PagedResultDto<AuditLogListItemDto>>
    {
        public string? Keyword { get; set; }

        public string? Module { get; set; }

        public string? Action { get; set; }

        public AuditActionType? ActionType { get; set; }

        public AuditStatus? Status { get; set; }

        public Guid? UserId { get; set; }

        public string? EntityName { get; set; }

        public Guid? EntityId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetAuditLogsQueryHandler
     : IRequestHandler<GetAuditLogsQuery, PagedResultDto<AuditLogListItemDto>>
    {
        private readonly IAuditLogService _auditLogService;

        public GetAuditLogsQueryHandler(
            IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<PagedResultDto<AuditLogListItemDto>> Handle(
            GetAuditLogsQuery request,
            CancellationToken cancellationToken)
        {
            var filter = new AuditLogFilterDto
            {
                Keyword = request.Keyword,
                Module = request.Module,
                Action = request.Action,
                ActionType = request.ActionType,
                Status = request.Status,
                UserId = request.UserId,
                EntityName = request.EntityName,
                EntityId = request.EntityId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            return await _auditLogService.GetPagedAsync(filter, cancellationToken);
        }
    }
}
