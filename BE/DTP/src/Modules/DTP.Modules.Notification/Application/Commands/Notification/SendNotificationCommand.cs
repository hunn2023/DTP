using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Commands.Notification
{
    public class SendNotificationCommand : IRequest<Guid>
    {
        public CreateNotificationRequest Request { get; set; } = default!;
    }

    public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, Guid>
    {
        private readonly INotificationService _notificationService;

        public SendNotificationCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<Guid> Handle(
            SendNotificationCommand request,
            CancellationToken cancellationToken)
        {
            return await _notificationService.SendAsync(
                request.Request,
                cancellationToken);
        }
    }
}
