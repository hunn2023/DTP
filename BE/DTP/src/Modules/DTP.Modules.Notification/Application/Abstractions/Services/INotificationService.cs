using DTP.Modules.Notification.Application.DTOs;
using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Abstractions.Services
{
    public interface INotificationService
    {
        Task<Guid> SendAsync(
            CreateNotificationRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<List<NotificationMessageDto>> GetMyNotificationsAsync(
            Guid userId,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<List<NotificationMessageDto>> GetAdminNotificationsAsync(
            Guid? userId,
            NotificationType? type,
            NotificationChannel? channel,
            NotificationStatus? status,
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<Guid> CreateTemplateAsync(
            CreateTemplateRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateTemplateAsync(
            Guid id,
            UpdateTemplateRequest request,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteTemplateAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<NotificationTemplateDto>> GetTemplatesAsync(
            CancellationToken cancellationToken = default);
    }
}
