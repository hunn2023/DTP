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
    public class NotificationMessageRepository : INotificationMessageRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationMessageRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationMessage?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _context.NotificationMessages
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        }

        public async Task<List<NotificationMessage>> GetByUserIdAsync(
            Guid userId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _context.NotificationMessages
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<NotificationMessage>> GetPagedAsync(
            Guid? userId,
            NotificationType? type,
            NotificationChannel? channel,
            NotificationStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.NotificationMessages
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (type.HasValue)
                query = query.Where(x => x.Type == type.Value);

            if (channel.HasValue)
                query = query.Where(x => x.Channel == channel.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountAsync(
            Guid? userId,
            NotificationType? type,
            NotificationChannel? channel,
            NotificationStatus? status,
            CancellationToken cancellationToken = default)
        {
            var query = _context.NotificationMessages
                .Where(x => !x.IsDeleted);

            if (userId.HasValue)
                query = query.Where(x => x.UserId == userId.Value);

            if (type.HasValue)
                query = query.Where(x => x.Type == type.Value);

            if (channel.HasValue)
                query = query.Where(x => x.Channel == channel.Value);

            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);

            return await query.CountAsync(cancellationToken);
        }

        public async Task AddAsync(
            NotificationMessage entity,
            CancellationToken cancellationToken = default)
        {
            await _context.NotificationMessages.AddAsync(entity, cancellationToken);
        }

        public void Update(NotificationMessage entity)
        {
            _context.NotificationMessages.Update(entity);
        }
    }
}
