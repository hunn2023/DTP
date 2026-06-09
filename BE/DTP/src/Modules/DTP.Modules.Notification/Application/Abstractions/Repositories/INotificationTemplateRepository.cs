using DTP.Modules.Notification.Domain.Entities;
using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Abstractions.Repositories
{
    public interface INotificationTemplateRepository
    {
        Task<NotificationTemplate?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<NotificationTemplate?> GetByCodeAsync(
            string code,
            CancellationToken cancellationToken = default);

        Task<NotificationTemplate?> GetActiveAsync(
            string code,
            NotificationChannel channel,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsCodeAsync(
            string code,
            Guid? excludeId = null,
            CancellationToken cancellationToken = default);

        Task<List<NotificationTemplate>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task AddAsync(
            NotificationTemplate entity,
            CancellationToken cancellationToken = default);

        void Update(NotificationTemplate entity);

        void Remove(NotificationTemplate entity);
    }
}
