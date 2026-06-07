using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.Queries
{
    public class GetAuditLogByIdQuery : IRequest<AuditLogDto?>
    {
        public GetAuditLogByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }

    public class GetAuditLogByIdQueryHandler
       : IRequestHandler<GetAuditLogByIdQuery, AuditLogDto?>
    {
        private readonly IAuditLogService _auditLogService;

        public GetAuditLogByIdQueryHandler(
            IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<AuditLogDto?> Handle(
            GetAuditLogByIdQuery request,
            CancellationToken cancellationToken)
        {
            return await _auditLogService.GetByIdAsync(
                request.Id,
                cancellationToken);
        }
    }
}
