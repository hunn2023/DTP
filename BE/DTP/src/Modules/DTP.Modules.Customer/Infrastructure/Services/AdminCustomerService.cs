using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Domain.Enums;
using DTP.Modules.Auth.Infrastructure.Persistence;
using DTP.Modules.Customer.Application.Abstractions.Services;
using DTP.Modules.Customer.Application.Constants;
using DTP.Modules.Customer.Application.DTOs;
using DTP.Shared.Application;
using Microsoft.EntityFrameworkCore;

namespace DTP.Modules.Customer.Infrastructure.Services
{
    public class AdminCustomerService : IAdminCustomerService
    {
        private readonly AuthDbContext _authDbContext;
        private readonly IAuditLogWriter _auditLogWriter;

        public AdminCustomerService(
            AuthDbContext authDbContext,
            IAuditLogWriter auditLogWriter)
        {
            _authDbContext = authDbContext;
            _auditLogWriter = auditLogWriter;
        }

        public async Task<Result<CustomerStatusResultDto>> UpdateStatusAsync(
            Guid userId,
            bool isActive,
            string? reason,
            Guid updatedByUserId,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                return Result<CustomerStatusResultDto>.Failure("UserId không hợp lệ.");

            if (updatedByUserId == Guid.Empty)
                return Result<CustomerStatusResultDto>.Failure("Không xác định được admin đang thao tác.");

            if (userId == updatedByUserId && !isActive)
                return Result<CustomerStatusResultDto>.Failure("Bạn không thể tự khóa tài khoản của chính mình.");

            var ip = string.IsNullOrWhiteSpace(ipAddress)
                ? "unknown"
                : ipAddress.Trim();

            var user = await _authDbContext.Users
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                await WriteAuditSafeAsync(
                    action: isActive ? "Unlock Customer Failed" : "Lock Customer Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: userId,
                    description: "Update customer status failed because user was not found.",
                    newValues: new
                    {
                        UserId = userId,
                        IsActive = isActive,
                        Reason = reason,
                        UpdatedByUserId = updatedByUserId,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result<CustomerStatusResultDto>.Failure("Không tìm thấy tài khoản.");
            }

            var isCustomer = user.UserRoles.Any(x => x.Role.Code == CustomerRoleCodes.Customer);

            if (!isCustomer)
            {
                await WriteAuditSafeAsync(
                    action: isActive ? "Unlock Customer Failed" : "Lock Customer Failed",
                    actionType: AuditActionType.Update,
                    status: AuditStatus.Failed,
                    entityName: "User",
                    entityId: user.Id,
                    description: "Update customer status failed because user is not CUSTOMER.",
                    newValues: new
                    {
                        user.Id,
                        user.Email,
                        Roles = user.UserRoles.Select(x => x.Role.Code).ToList(),
                        IsActive = isActive,
                        Reason = reason,
                        UpdatedByUserId = updatedByUserId,
                        IpAddress = ip,
                        UserAgent = userAgent,
                        FailedAt = DateTime.UtcNow
                    },
                    cancellationToken: cancellationToken);

                return Result<CustomerStatusResultDto>.Failure("Tài khoản này không phải khách hàng.");
            }

            var oldStatus = user.IsActive;

            if (oldStatus == isActive)
            {
                return Result<CustomerStatusResultDto>.Success(new CustomerStatusResultDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    IsActive = user.IsActive
                });
            }

            user.IsActive = isActive;

            await _authDbContext.SaveChangesAsync(cancellationToken);

            await WriteAuditSafeAsync(
                action: isActive ? "Unlock Customer Success" : "Lock Customer Success",
                actionType: AuditActionType.Update,
                status: AuditStatus.Success,
                entityName: "User",
                entityId: user.Id,
                description: isActive
                    ? "Customer account was unlocked."
                    : "Customer account was locked.",
                oldValues: new
                {
                    user.Id,
                    user.Email,
                    IsActive = oldStatus
                },
                newValues: new
                {
                    user.Id,
                    user.Email,
                    user.FullName,
                    IsActive = user.IsActive,
                    Reason = reason,
                    UpdatedByUserId = updatedByUserId,
                    IpAddress = ip,
                    UserAgent = userAgent,
                    UpdatedAt = DateTime.UtcNow
                },
                cancellationToken: cancellationToken);

            return Result<CustomerStatusResultDto>.Success(new CustomerStatusResultDto
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive
            });
        }

        private async Task WriteAuditSafeAsync(
            string action,
            AuditActionType actionType,
            AuditStatus status,
            string? entityName = null,
            Guid? entityId = null,
            string? description = null,
            object? oldValues = null,
            object? newValues = null,
            string? errorMessage = null,
            CancellationToken cancellationToken = default)
        {
            await _auditLogWriter.WriteAsync(
                module: "Customer",
                action: action,
                actionType: actionType,
                status: status,
                entityName: entityName,
                entityId: entityId,
                description: description,
                oldValues: oldValues,
                newValues: newValues,
                errorMessage: errorMessage,
                cancellationToken: cancellationToken);
        }
    }
}
