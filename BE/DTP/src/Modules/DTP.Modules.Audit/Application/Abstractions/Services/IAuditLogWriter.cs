using DTP.Modules.Audit.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.Abstractions.Services
{
    public interface IAuditLogWriter
    {
        Task WriteAsync(
            string module,
            string action,
            AuditActionType actionType,
            AuditStatus status = AuditStatus.Success,
            string? entityName = null,
            Guid? entityId = null,
            string? description = null,
            object? oldValues = null,
            object? newValues = null,
            string? errorMessage = null,
            CancellationToken cancellationToken = default);
    }
}
