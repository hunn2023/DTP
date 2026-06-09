using DTP.Modules.Audit.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.DTOs
{
    public class AuditLogFilterDto
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
}
