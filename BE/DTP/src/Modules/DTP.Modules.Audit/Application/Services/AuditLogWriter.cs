using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.DTOs;
using DTP.Modules.Audit.Domain.Enums;
using System.Text.Json;

namespace DTP.Modules.Audit.Application.Services
{
    public class AuditLogWriter : IAuditLogWriter
    {
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentAuditUserService _currentUser;

        public AuditLogWriter(
            IAuditLogService auditLogService,
            ICurrentAuditUserService currentUser)
        {
            _auditLogService = auditLogService;
            _currentUser = currentUser;
        }

        public async Task WriteAsync(
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
            CancellationToken cancellationToken = default)
        {
            var dto = new CreateAuditLogDto
            {
                Module = module,
                Action = action,
                ActionType = actionType,
                Status = status,

                UserId = _currentUser.UserId,
                UserName = _currentUser.UserName,

                EntityName = entityName,
                EntityId = entityId,

                Description = description,
                OldValues = SerializeObject(oldValues),
                NewValues = SerializeObject(newValues),

                IpAddress = _currentUser.IpAddress,
                UserAgent = _currentUser.UserAgent,
                RequestPath = _currentUser.RequestPath,
                RequestMethod = _currentUser.RequestMethod,
                CorrelationId = _currentUser.CorrelationId,

                ErrorMessage = errorMessage
            };

            await _auditLogService.CreateAsync(dto, cancellationToken);
        }

        private static string? SerializeObject(object? value)
        {
            if (value == null)
                return null;

            return JsonSerializer.Serialize(
                value,
                new JsonSerializerOptions
                {
                    WriteIndented = false
                });
        }
    }
}
