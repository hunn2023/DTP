using DTP.Modules.Notification.Domain.Entities;
using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Abstractions.Repositories
{
    public interface INotificationMessageRepository
    {
        Task<NotificationMessage?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<NotificationMessage>> GetByUserIdAsync(
            Guid userId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<List<NotificationMessage>> GetPagedAsync(
            Guid? userId,
            NotificationType? type,
            NotificationChannel? channel,
            NotificationStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<int> CountAsync(
            Guid? userId,
            NotificationType? type,
            NotificationChannel? channel,
            NotificationStatus? status,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            NotificationMessage entity,
            CancellationToken cancellationToken = default);

        void Update(NotificationMessage entity);
    }
}
