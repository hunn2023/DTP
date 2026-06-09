using DTP.Modules.Audit.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.DTOs
{
    public class AuditLogListItemDto
    {
        public Guid Id { get; set; }

        public string Module { get; set; } = default!;

        public string Action { get; set; } = default!;

        public AuditActionType ActionType { get; set; }

        public AuditStatus Status { get; set; }

        public Guid? UserId { get; set; }

        public string? UserName { get; set; }

        public string? EntityName { get; set; }

        public Guid? EntityId { get; set; }

        public string? IpAddress { get; set; }

        public string? RequestPath { get; set; }

        public string? RequestMethod { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
