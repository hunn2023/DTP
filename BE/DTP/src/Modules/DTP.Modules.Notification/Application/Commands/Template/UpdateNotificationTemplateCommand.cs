using DTP.Modules.Notification.Application.Abstractions.Services;
using DTP.Modules.Notification.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Notification.Application.Commands.Template
{
    public class UpdateNotificationTemplateCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

        public UpdateTemplateRequest Request { get; set; } = default!;
    }

    public class UpdateNotificationTemplateCommandHandler : IRequestHandler<UpdateNotificationTemplateCommand, bool>
    {
        private readonly INotificationService _notificationService;

        public UpdateNotificationTemplateCommandHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(
            UpdateNotificationTemplateCommand request,
            CancellationToken cancellationToken)
        {
            return await _notificationService.UpdateTemplateAsync(
                request.Id,
                request.Request,
                cancellationToken);
        }
    }
}
