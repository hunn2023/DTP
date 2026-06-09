using DTP.Modules.Audit.Application.DTOs;
using DTP.Modules.Audit.Domain.Entities;
using DTP.Shared.Application.Pagination;


namespace DTP.Modules.Audit.Application.Abstractions.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(
            AuditLog auditLog,
            CancellationToken cancellationToken = default);

        Task<AuditLog?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<AuditLogListItemDto>> GetPagedAsync(
            AuditLogFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
