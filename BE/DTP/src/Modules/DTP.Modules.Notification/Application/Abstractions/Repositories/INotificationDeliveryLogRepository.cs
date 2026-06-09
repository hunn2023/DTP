using DTP.Modules.Notification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Abstractions.Repositories
{
    public interface INotificationDeliveryLogRepository
    {
        Task AddAsync(
            NotificationDeliveryLog entity,
            CancellationToken cancellationToken = default);

        Task<List<NotificationDeliveryLog>> GetByNotificationIdAsync(
            Guid notificationMessageId,
            CancellationToken cancellationToken = default);
    }
}
