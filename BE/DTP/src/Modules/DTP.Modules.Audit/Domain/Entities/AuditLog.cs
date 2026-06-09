using DTP.Modules.Audit.Domain.Enums;
using DTP.Shared.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Domain.Entities
{
    public class AuditLog : EntityBase
    {
        private AuditLog()
        {
        }

        public AuditLog(
            string module,
            string action,
            AuditActionType actionType,
            AuditStatus status,
            Guid? userId,
            string? userName,
            string? entityName,
            Guid? entityId,
            string? description,
            string? oldValues,
            string? newValues,
            string? ipAddress,
            string? userAgent,
            string? requestPath,
            string? requestMethod,
            string? correlationId,
            string? errorMessage)
        {
            Id = Guid.NewGuid();

            Module = module;
            Action = action;
            ActionType = actionType;
            Status = status;

            UserId = userId;
            UserName = userName;

            EntityName = entityName;
            EntityId = entityId;

            Description = description;
            OldValues = oldValues;
            NewValues = newValues;

            IpAddress = ipAddress;
            UserAgent = userAgent;
            RequestPath = requestPath;
            RequestMethod = requestMethod;
            CorrelationId = correlationId;

            ErrorMessage = errorMessage;

            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }

        public string Module { get; private set; } = default!;

        public string Action { get; private set; } = default!;

        public AuditActionType ActionType { get; private set; }

        public AuditStatus Status { get; private set; }

        public Guid? UserId { get; private set; }

        public string? UserName { get; private set; }

        public string? EntityName { get; private set; }

        public Guid? EntityId { get; private set; }

        public string? Description { get; private set; }

        public string? OldValues { get; private set; }

        public string? NewValues { get; private set; }

        public string? IpAddress { get; private set; }

        public string? UserAgent { get; private set; }

        public string? RequestPath { get; private set; }

        public string? RequestMethod { get; private set; }

        public string? CorrelationId { get; private set; }

        public string? ErrorMessage { get; private set; }
    }
}
