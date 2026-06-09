using DTP.Modules.Notification.Application.Abstractions.Repositories;
using DTP.Modules.Notification.Domain.Entities;
using DTP.Modules.Notification.Domain.Enums;
using DTP.Modules.Notification.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Infrastructure.Repositories
{
    public class NotificationTemplateRepository : INotificationTemplateRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationTemplateRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationTemplate?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<NotificationTemplate?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(x => x.Code == code && !x.IsDeleted, cancellationToken);
        }

        public async Task<NotificationTemplate?> GetActiveAsync(
            string code,
            NotificationChannel channel,
            CancellationToken cancellationToken = default)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(
                    x => x.Code == code
                         && x.Channel == channel
                         && x.IsActive
                         && !x.IsDeleted,
                    cancellationToken);
        }

        public async Task<bool> ExistsCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.NotificationTemplates
                .Where(x => x.Code == code && !x.IsDeleted);

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<List<NotificationTemplate>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.NotificationTemplates
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            NotificationTemplate entity,
            CancellationToken cancellationToken = default)
        {
            await _context.NotificationTemplates.AddAsync(entity, cancellationToken);
        }

        public void Update(NotificationTemplate entity)
        {
            _context.NotificationTemplates.Update(entity);
        }

        public void Remove(NotificationTemplate entity)
        {
            entity.Delete();
            _context.NotificationTemplates.Update(entity);
        }
    }
}
