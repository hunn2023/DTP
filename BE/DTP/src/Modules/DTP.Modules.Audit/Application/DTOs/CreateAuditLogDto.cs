using DTP.Modules.Audit.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.DTOs
{
    public class CreateAuditLogDto
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
}
