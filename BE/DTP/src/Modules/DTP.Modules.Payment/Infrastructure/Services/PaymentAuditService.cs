using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Payment.Application.Abstractions.Services;


namespace DTP.Modules.Payment.Infrastructure.Services
{
    public class PaymentAuditService : IPaymentAuditService
    {
        private readonly IAuditLogWriter _auditLogService;

        public PaymentAuditService(IAuditLogWriter auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task WriteAsync(
            string action,
            string status,
            Guid? entityId,
            string description,
            object? oldValues,
            object? newValues,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var auditStatus = status.Equals("Success", StringComparison.OrdinalIgnoreCase)
                    ? AuditStatus.Success
                    : AuditStatus.Failed;

                await _auditLogService.WriteAsync(
                    module: "Payment",
                    action: action,
                    actionType: AuditActionType.Payment,
                    status: auditStatus,
                    entityName: "PaymentTransaction",
                    entityId: entityId,
                    description: description,
                    oldValues: oldValues,
                    newValues: newValues,
                    cancellationToken: cancellationToken);
            }
            catch
            {
                // Không throw audit error để tránh làm hỏng flow thanh toán.
            }
        }
    }
}
