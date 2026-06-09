using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Queries.Notifications
{
    public class GetMyNotificationsQuery : IRequest<List<NotificationMessageDto>>
    {
        public Guid UserId { get; set; }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }

    public class GetMyNotificationsQueryHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationMessageDto>>
    {
        private readonly INotificationService _notificationService;

        public GetMyNotificationsQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<List<NotificationMessageDto>> Handle(
            GetMyNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            return await _notificationService.GetMyNotificationsAsync(
                request.UserId,
                request.PageIndex,
                request.PageSize,
                cancellationToken);
        }
    }
}
