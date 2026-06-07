using DTP.Modules.Notification.Application.Abstractions.Repositories;
using DTP.Modules.Notification.Domain.Entities;
using DTP.Modules.Notification.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Infrastructure.Repositories
{
    public class NotificationDeliveryLogRepository : INotificationDeliveryLogRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationDeliveryLogRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            NotificationDeliveryLog entity,
            CancellationToken cancellationToken = default)
        {
            await _context.NotificationDeliveryLogs.AddAsync(entity, cancellationToken);
        }

        public async Task<List<NotificationDeliveryLog>> GetByNotificationIdAsync(
            Guid notificationMessageId,
            CancellationToken cancellationToken = default)
        {
            return await _context.NotificationDeliveryLogs
                .AsNoTracking()
                .Where(x => x.NotificationMessageId == notificationMessageId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
