using DTP.Modules.Audit.Application.Abstractions.Repositories;
using DTP.Modules.Audit.Application.Abstractions.Services;
using DTP.Modules.Audit.Application.Abstractions.UnitOfWork;
using DTP.Modules.Audit.Application.DTOs;
using DTP.Modules.Audit.Domain.Entities;
using DTP.Shared.Application;
using DTP.Shared.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Audit.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAuditUnitOfWork _unitOfWork;

        public AuditLogService(
            IAuditLogRepository auditLogRepository,
            IAuditUnitOfWork unitOfWork)
        {
            _auditLogRepository = auditLogRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateAsync(
            CreateAuditLogDto dto,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Module))
                throw new ArgumentException("Module is required.");

            if (string.IsNullOrWhiteSpace(dto.Action))
                throw new ArgumentException("Action is required.");

            var auditLog = new AuditLog(
                dto.Module.Trim(),
                dto.Action.Trim(),
                dto.ActionType,
                dto.Status,
                dto.UserId,
                dto.UserName,
                dto.EntityName,
                dto.EntityId,
                dto.Description,
                dto.OldValues,
                dto.NewValues,
                dto.IpAddress,
                dto.UserAgent,
                dto.RequestPath,
                dto.RequestMethod,
                dto.CorrelationId,
                dto.ErrorMessage);

            await _auditLogRepository.AddAsync(auditLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return auditLog.Id;
        }

        public async Task<AuditLogDto?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var auditLog = await _auditLogRepository.GetByIdAsync(id, cancellationToken);

            if (auditLog == null)
                return null;

            return new AuditLogDto
            {
                Id = auditLog.Id,
                Module = auditLog.Module,
                Action = auditLog.Action,
                ActionType = auditLog.ActionType,
                Status = auditLog.Status,
                UserId = auditLog.UserId,
                UserName = auditLog.UserName,
                EntityName = auditLog.EntityName,
                EntityId = auditLog.EntityId,
                Description = auditLog.Description,
                OldValues = auditLog.OldValues,
                NewValues = auditLog.NewValues,
                IpAddress = auditLog.IpAddress,
                UserAgent = auditLog.UserAgent,
                RequestPath = auditLog.RequestPath,
                RequestMethod = auditLog.RequestMethod,
                CorrelationId = auditLog.CorrelationId,
                ErrorMessage = auditLog.ErrorMessage,
                CreatedAt = auditLog.CreatedAt
            };
        }

        public async Task<Result<PagedResultDto<AuditLogListItemDto>>> GetPagedAsync(
            AuditLogFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            if (filter.PageIndex <= 0)
                filter.PageIndex = 1;

            if (filter.PageSize <= 0)
                filter.PageSize = 20;

            if (filter.PageSize > 100)
                filter.PageSize = 100;

            var result = await _auditLogRepository.GetPagedAsync(filter, cancellationToken);

            return Result<PagedResultDto<AuditLogListItemDto>>.Success(result);
        }
    }
}
