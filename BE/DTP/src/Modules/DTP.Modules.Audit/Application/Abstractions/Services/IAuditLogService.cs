using DTP.Modules.Audit.Application.DTOs;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.Abstractions.Services
{
    public interface IAuditLogService
    {
        Task<Guid> CreateAsync(
            CreateAuditLogDto dto,
            CancellationToken cancellationToken = default);

        Task<AuditLogDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<PagedResultDto<AuditLogListItemDto>> GetPagedAsync(
            AuditLogFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
