using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Application.DTOs;
using DTP.Modules.Notification.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Queries.Notifications
{
    public class GetAdminNotificationsQuery : IRequest<List<NotificationMessageDto>>
    {
        public Guid? UserId { get; set; }

        public NotificationType? Type { get; set; }

        public NotificationChannel? Channel { get; set; }

        public NotificationStatus? Status { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetAdminNotificationsQueryHandler : IRequestHandler<GetAdminNotificationsQuery, List<NotificationMessageDto>>
    {
        private readonly INotificationService _notificationService;

        public GetAdminNotificationsQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<List<NotificationMessageDto>> Handle(
            GetAdminNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            return await _notificationService.GetAdminNotificationsAsync(
                request.UserId,
                request.Type,
                request.Channel,
                request.Status,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
