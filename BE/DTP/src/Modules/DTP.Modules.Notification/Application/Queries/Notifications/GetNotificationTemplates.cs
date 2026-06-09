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
    public class GetNotificationTemplatesQuery : IRequest<List<NotificationTemplateDto>>
    {
    }

    public class GetNotificationTemplatesQueryHandler : IRequestHandler<GetNotificationTemplatesQuery, List<NotificationTemplateDto>>
    {
        private readonly INotificationService _notificationService;

        public GetNotificationTemplatesQueryHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<List<NotificationTemplateDto>> Handle(
            GetNotificationTemplatesQuery request,
            CancellationToken cancellationToken)
        {
            return await _notificationService.GetTemplatesAsync(cancellationToken);
        }
    }
}
